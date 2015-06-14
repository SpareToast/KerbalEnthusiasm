using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KerbalEnthusiasm
{
    class KerbalEnthusiasm
    {
        double BaseEnthusiasm = 1.0;
        double MinEnthusiasm = 0;
        double MaxEnthusiasm = 100;
        double DefaultEnthusiasm = 100;
        double SoIChange = 25;

        List<EnthusiasmStatus> trackedKerbals;
        List<VesselQuality> trackedVessels;

        
        public void UpdatePopulation(Vessel vessel)
        {
            // cycles through each crewmember on a ship for updates
            foreach (ProtoCrewMember kerbal in vessel.GetVesselCrew())
            {
                EnthusiasmStatus oldKerbalStatus = GetEnthusiasmStatus(vessel, kerbal);
                trackedKerbals.Remove(oldKerbalStatus);
                VesselQuality oldVessel = trackedVessels.FirstOrDefault(v => v.VesselId == oldKerbalStatus.VesselID);
                EnthusiasmStatus newKerbalStatus = UpdateEnthusiasm(kerbal, vessel, oldKerbalStatus, oldVessel);
                trackedKerbals.Add(newKerbalStatus);
            }

        }

        // If the kerbal has no status entry, make one. Otherwise, return kerbal's status
        public EnthusiasmStatus GetEnthusiasmStatus(Vessel vessel, ProtoCrewMember kerbal)
        {
            EnthusiasmStatus kerbalStatus = trackedKerbals.FirstOrDefault(x => x.KerbalName == kerbal.name);
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

            return kerbalStatus;
        }

        // If the vessel has no quality entry, make one. Otherwise, return vessel's quality
        public VesselQuality GetVesselQuality(Vessel vessel)
        {
            VesselQuality vesselQuality = trackedVessels.FirstOrDefault(x => x.VesselId == vessel.id);
            if (vesselQuality == null)
            {
                vesselQuality.VesselId = vessel.id;
                vesselQuality.VesselName = vessel.name;
                vesselQuality.LastUpdate = Planetarium.GetUniversalTime();
                vesselQuality.PopulationSize = vessel.GetVesselCrew().Count;
                vesselQuality.TotalSpace =  vessel.GetCrewCapacity();
                vesselQuality.FreeSpace = vesselQuality.TotalSpace - vesselQuality.PopulationSize;
                vesselQuality.Morale = UpdateVesselQuality(vessel);
            }
            return vesselQuality;
        }

        //Returns a double between .75 and 1.25 showing current vessel morale modifier
        //Todo: Include more things than current pop, likepilots, spare seats etc.
        //Todo: Split into subfunctions 
        double UpdateVesselQuality(Vessel vessel)
        {
            int vPopSize = VesselPopulationSize(vessel);
            int vMaxCap = VesselMaxCapacity(vessel);
            int vSpace = vMaxCap * 2;
            double qMin = 0.75;
            double qMax = 1.25;
            //            int badasses = VesselBaddasses();
            //            int pilots = VesselPilots();
            int company = (vPopSize - 1) / (2 * vPopSize);
            // double legRoom = vSpace - vPopSize / 2 * vSpace;
            // double Leadership = badasses + pilots;
            double quality = Math.Min((qMin + company), qMax);
            return quality;
        }
    }
}
