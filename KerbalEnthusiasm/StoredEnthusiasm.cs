using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KerbalEnthusiasm
{
    class StoredEnthusiasm
    {
        double BaseEnthusiasm = 1.0;
        double MinEnthusiasm = 0;
        double MaxEnthusiasm = 100;
        double DefaultEnthusiasm = 100;
        double SoIChange = 25;

        private static Dictionary<String, EnthusiasmStatus> trackedKerbals;
        private static Dictionary<Guid, VesselQuality> trackedVessels;

        public void PopulateLists()
        {
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.loaded)
                {
                    trackedVessels.Add(vessel.id, GetVesselQuality (vessel.id, vessel));
                    foreach (ProtoCrewMember kerbal in vessel.GetVesselCrew())
                    {
                        trackedKerbals.Add(kerbal.name, GetEnthusiasmStatus(vessel, kerbal));
                    }
                }

            }
        }

        public void UpdatePopulation(Vessel vessel)
        {
            // cycles through each crewmember on a ship for updates
            foreach (ProtoCrewMember kerbal in vessel.GetVesselCrew())
            {

                EnthusiasmStatus oldKerbalStatus = GetEnthusiasmStatus(vessel, kerbal);
                trackedKerbals.Remove(oldKerbalStatus.KerbalName);

                VesselQuality oldVesselQuality = GetVesselQuality(oldKerbalStatus.VesselID, vessel);
                
                EnthusiasmStatus newKerbalStatus = UpdateEnthusiasm(kerbal, vessel, oldKerbalStatus, oldVesselQuality);
                trackedKerbals.Add(newKerbalStatus.KerbalName, newKerbalStatus);
            }

        }

        // If the kerbal has no status entry, make one. Otherwise, return kerbal's status
        public EnthusiasmStatus GetEnthusiasmStatus(Vessel vessel, ProtoCrewMember kerbal)
        {
            EnthusiasmStatus kerbalStatus = trackedKerbals[kerbal.name];
            if (kerbalStatus == null)
            {
                kerbalStatus.KerbalName = kerbal.name;
                kerbalStatus.VesselID = vessel.id;
                kerbalStatus.Enthusiasm = DefaultEnthusiasm;
                kerbalStatus.LastUpdate = Planetarium.GetUniversalTime();
                kerbalStatus.LastSOI = vessel.mainBody.name;
            }
            return kerbalStatus;
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
                kerbalStatus.Enthusiasm = Math.Min((kerbalStatus.Enthusiasm + 25), MaxEnthusiasm);
            }

            if (kerbalStatus.VesselID != vessel.id)
            {
                kerbalStatus.VesselID = vessel.id;
            }

            //set the Kerbal's stored ship to be the ship it is now on
            kerbalStatus.VesselID = vessel.id;

            return kerbalStatus;
        }

        // If the vessel has no quality entry, make one. Otherwise, return vessel's quality
        public VesselQuality GetVesselQuality(Guid VesselID, Vessel vessel)
        {
            VesselQuality vesselQuality = trackedVessels[VesselID];
            if (vesselQuality == null)
            {
                vesselQuality.VesselId = vessel.id;
                vesselQuality.VesselName = vessel.name;
                vesselQuality.LastUpdate = Planetarium.GetUniversalTime();
                vesselQuality.PopulationSize = vessel.GetVesselCrew().Count;
                vesselQuality.TotalSpace =  vessel.GetCrewCapacity();
                vesselQuality.FreeSpace = vesselQuality.TotalSpace - vesselQuality.PopulationSize;
                vesselQuality.Morale = CalculateVesselQuality(vesselQuality);
            }
            return vesselQuality;
        }

        VesselQuality UpdateVesselQuality(Vessel vessel)
        {
            VesselQuality newVesselQuality = new VesselQuality;
            newVesselQuality.VesselId = vessel.id;
            newVesselQuality.VesselName = vessel.name;
            newVesselQuality.LastUpdate = Planetarium.GetUniversalTime();
            newVesselQuality.PopulationSize = vessel.GetVesselCrew().Count;
            newVesselQuality.TotalSpace =  vessel.GetCrewCapacity();
            newVesselQuality.FreeSpace = newVesselQuality.TotalSpace - newVesselQuality.PopulationSize;
            newVesselQuality.Morale = CalculateVesselQuality(newVesselQuality);
            return newVesselQuality;
        }

        //Returns a double between .75 and 1.25 showing current vessel morale modifier
        //Todo: Include more things than current pop, likepilots, spare seats etc.
        //Todo: Split into subfunctions 
        double CalculateVesselQuality(VesselQuality vesselQuality)
        {
            int vPopSize = vesselQuality.PopulationSize;
            int vMaxCap = vesselQuality.TotalSpace;
            int vSpace = vMaxCap * 2;
            double qMin = 0.67;
            double qMax = 1.34;
            //            int badasses = VesselBaddasses();
            //            int pilots = VesselPilots();
            int company = (vPopSize - 1) / (2 * vPopSize);
            double legRoom = (vSpace - vPopSize + 2) / 2 * (vSpace - vPopSize + 1);
            // double Leadership = badasses + pilots;
            double quality = Math.Min((qMin + company + legRoom), qMax);
            return quality;
        }
    }
}
