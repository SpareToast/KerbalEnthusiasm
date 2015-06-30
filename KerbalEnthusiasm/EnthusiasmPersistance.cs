using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace KerbalEnthusiasm
{
    //Much of the below code blatantly "inspired" by Roverdude's LifeSupport

    public class EnthusiasmPersistance : ScenarioModule
    {

        public ConfigNode ModNode { get; private set; }

        public static EnthusiasmPersistance Instance { get; private set; }


        private static Dictionary<Guid, VesselQuality> _TrackedVessels;
        private static Dictionary<String, KerbalStatus> _TrackedKerbals;

        public static Dictionary<String, KerbalStatus> TrackedKerbals(){
            return _TrackedKerbals;
        }

        public static Dictionary<Guid, VesselQuality> TrackedVessels(){
            return _TrackedVessels;
        }

        public EnthusiasmPersistance(){
            Instance = this;
        }

        public override void OnLoad(ConfigNode gameNode)
        {   
            base.OnLoad(gameNode);
            if (gameNode.HasNode("KERBAL_ENTHUSIASM"))
            {
                ModNode = gameNode.GetNode("KERBAL_ENTHUSIASM");
                ConfigNode[] vNodes = ModNode.GetNodes("VESSELQUALITY");
                ConfigNode[] kNodes = ModNode.GetNodes("KERBALSTATUS");

                Dictionary<Guid, VesselQuality> vDict = new Dictionary<Guid, VesselQuality>();
                Dictionary<String, KerbalStatus> kDict = new Dictionary<String, KerbalStatus>();

                foreach (ConfigNode vNode in vNodes)
                {
                    VesselQuality v = ResourceUtilities.LoadNodeProperties<VesselQuality>(vNode);
                    vDict.Add(v.VesselId, v);
                }

                foreach (ConfigNode kNode in kNodes)
                {
                    KerbalStatus k = ResourceUtilities.LoadNodeProperties<KerbalStatus>(kNode);
                    kDict.Add(k.KerbalName, k);
                }
            }
            else
            {
                _TrackedVessels = new Dictionary<Guid, VesselQuality>();
                _TrackedKerbals = new Dictionary<String, KerbalStatus>();
            
            }
        }

        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);

            if (gameNode.HasNode("KERBAL_ENTHUSIASM")) {
                ModNode = gameNode.GetNode("KERBAL_ENTHUSIASM");
            }
            else {
                ModNode = gameNode.AddNode("KERBAL_ENTHUSIASM");
            }

            foreach (KeyValuePair<Guid, VesselQuality> v in _TrackedVessels)
            {
                ConfigNode vNode = new ConfigNode("VESSELQUALITY");
                vNode.AddValue("VesselId", v.Value.VesselId);
                vNode.AddValue("VesselName", v.Value.VesselName);
                vNode.AddValue("LastUpdate", v.Value.LastUpdate);
                vNode.AddValue("PopulationSize", v.Value.PopulationSize);
                vNode.AddValue("TotalSpace", v.Value.TotalSpace);
                vNode.AddValue("FreeSpace", v.Value.FreeSpace);
                vNode.AddValue("Morale", v.Value.Morale);
                ModNode.AddNode(vNode);
            }

            foreach (KeyValuePair<string, KerbalStatus> k in _TrackedKerbals)
            {
                ConfigNode kNode = new ConfigNode("KERBALSTATUS");
                kNode.AddValue("KerbalName", k.Value.KerbalName);
                kNode.AddValue("VesselID", k.Value.VesselID);
                kNode.AddValue("Enthusiasm", k.Value.Enthusiasm);
                kNode.AddValue("LastUpdate", k.Value.LastUpdate);
                kNode.AddValue("LastSOI", k.Value.LastSOI);
                ModNode.AddNode(kNode);
            }
        }
    }
}