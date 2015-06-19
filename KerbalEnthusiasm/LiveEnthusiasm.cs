using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KerbalEnthusiasm
{
    class LiveEnthusiasm
    {
//todo: Add protections for freefloating kerbals
        
        double BaseEnthusiasm = 1.0;
        double MinEnthusiasm = 0;
        double MaxEnthusiasm = 100;
        double DefaultEnthusiasm = 100;
        double SoIChange = 25;

        private static Dictionary<String, EnthusiasmStatus> trackedKerbals = new Dictionary<string,EnthusiasmStatus>();
        private static Dictionary<Guid, VesselQuality> trackedVessels = new Dictionary<Guid,VesselQuality>();

        public Dictionary<String, EnthusiasmStatus> TrackedKerbals(){
            return trackedKerbals;
        }

        public Dictionary<Guid, VesselQuality> TrackedVessels(){
            return trackedVessels;
        }

        //Populate at game start
        public void PopulateLists() {
            Debug.Log("Populating!");
            int vessA = 0;
            int vessB = 0;
            int kerbC = 0;
            if ((trackedKerbals.Count == 0)||(trackedVessels.Count == 0)) {
                Debug.Log("Trying to populate" + FlightGlobals.Vessels.Count);
                foreach (Vessel vessel in FlightGlobals.Vessels) {
                    Debug.Log("Populated A " + vessA++);
                    if (vessel.loaded) {
                        Debug.Log("Populated B " + vessB++);
                        trackedVessels.Add(vessel.id, GetVesselQuality(vessel.id, vessel));
                        foreach (ProtoCrewMember kerbal in vessel.GetVesselCrew())
                        {
                            Debug.Log("Populated C " + kerbC++);
                            trackedKerbals.Add(kerbal.name, GetEnthusiasmStatus(kerbal.name, vessel));
                        }
                    }
                }
            }
        }

        // cycles through each crewmember on a vessel and updates them
        public void UpdatePopulation(Vessel vessel) {
            foreach (ProtoCrewMember kerbal in vessel.GetVesselCrew()) {

                EnthusiasmStatus oldKerbalStatus = GetEnthusiasmStatus(kerbal.name, vessel);
                trackedKerbals.Remove(oldKerbalStatus.KerbalName);

                VesselQuality oldVesselQuality = GetVesselQuality(oldKerbalStatus.VesselID, vessel);
                
                EnthusiasmStatus newKerbalStatus = UpdateEnthusiasm(kerbal, vessel, oldKerbalStatus, oldVesselQuality);
                trackedKerbals.Add(newKerbalStatus.KerbalName, newKerbalStatus);
            }
        }

        // If the kerbal has no status entry, make one. Otherwise, return kerbal's status
        public EnthusiasmStatus GetEnthusiasmStatus(String kerbalName, Vessel vessel) {
            EnthusiasmStatus kerbalStatus = trackedKerbals[kerbalName];
            if (kerbalStatus == null) {
                kerbalStatus.KerbalName = kerbalName;
                kerbalStatus.VesselID = vessel.id;
                kerbalStatus.Enthusiasm = DefaultEnthusiasm;
                kerbalStatus.LastUpdate = Planetarium.GetUniversalTime();
                kerbalStatus.LastSOI = vessel.mainBody.name;
            }
            return kerbalStatus;
        }

        // If the vessel has no quality entry, make one. Otherwise, return vessel's quality
        public VesselQuality GetVesselQuality(Guid VesselID, Vessel vessel) {
            VesselQuality vesselQuality = trackedVessels[VesselID];
            if (vesselQuality == null)
                vesselQuality = UpdateVesselQuality(vessel);
            return vesselQuality;
        }

        // Updates the kerbal's current enthusiasm
        // Todo: add more enthusiasm generators
        public EnthusiasmStatus UpdateEnthusiasm(ProtoCrewMember kerbal, Vessel vessel, EnthusiasmStatus kerbalStatus, VesselQuality vesselQuality)
        {
            double nowTime = Planetarium.GetUniversalTime();
            double moraleLoss = (BaseEnthusiasm / vesselQuality.Morale) * Time.deltaTime * (nowTime - kerbalStatus.LastUpdate);
            kerbalStatus.Enthusiasm = Math.Max((kerbalStatus.Enthusiasm - (BaseEnthusiasm / vesselQuality.Morale)), MinEnthusiasm);
            
            //changing SoI is exciting!
            if (kerbalStatus.LastSOI != vessel.mainBody.name)
            {
                kerbalStatus.Enthusiasm = Math.Min((kerbalStatus.Enthusiasm + SoIChange), MaxEnthusiasm);
            }
            
            //set the Kerbal's stored ship to be the ship it is on now
            if (kerbalStatus.VesselID != vessel.id)
            {
                kerbalStatus.VesselID = vessel.id;
            }

            return kerbalStatus;
        }

        //updates vessel's attributes
        VesselQuality UpdateVesselQuality(Vessel vessel) {
            VesselQuality newVesselQuality = new VesselQuality();
            
            newVesselQuality.VesselId = vessel.id;
            newVesselQuality.VesselName = vessel.name;
            newVesselQuality.LastUpdate = Planetarium.GetUniversalTime();
            newVesselQuality.PopulationSize = vessel.GetVesselCrew().Count;
            newVesselQuality.TotalSpace =  vessel.GetCrewCapacity();
            newVesselQuality.FreeSpace = newVesselQuality.TotalSpace - newVesselQuality.PopulationSize;
            newVesselQuality.Morale = CalculateVesselQuality(newVesselQuality);
            
            return newVesselQuality;
        }

        //Returns a double for current vessel morale modifier
        //Todo: Include more things like pilots, badasses etc.
        //Todo: Split into subfunctions?
        double CalculateVesselQuality(VesselQuality vesselQuality)
        {
            int vPop = vesselQuality.PopulationSize;
            int vSeats = vesselQuality.TotalSpace;
            int company = 0;
            int legRoom = 0;
//          int badasses = 0;
//          int pilots = 0;
            double quality = 1;
            double qMin = 0.67;
            double qMax = 1.34;

            if (vPop > 0) company = (vPop - 1) / (2 * vPop);
            if (vSeats > 0) legRoom = (vSeats - vPop) / (2 * (vSeats - vPop + 1));
//          double Leadership = badasses + pilots;
            quality = Math.Min((qMin + company + legRoom), qMax);
            return quality;
        }
    }
}
