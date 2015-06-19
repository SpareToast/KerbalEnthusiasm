using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KerbalEnthusiasm
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class EnthusiasmLoader : MonoBehaviour
    {
        // Todo: Actually load this into the game
        public EnthusiasmLoader()
        {
            Debug.Log("Enthusiasm is constructed");
        }
        
        void Awake()
        {
            Debug.Log("Enthusiasm is awake");
        }

        void Start()
        {
            Debug.Log("Enthusiasm is starting");
            LiveEnthusiasm liveEnthusiasm = new LiveEnthusiasm();
            Debug.Log("Did it make a thing?");
            liveEnthusiasm.PopulateLists();
            Debug.Log("Did it populate?");
            Debug.Log(liveEnthusiasm.TrackedVessels().Count());
            Debug.Log(liveEnthusiasm.TrackedKerbals().Count());
            foreach (VesselQuality v in liveEnthusiasm.TrackedVessels().Values.ToList())
            {
                Debug.Log("a Vessel");
                Debug.Log(v.VesselId);
                Debug.Log(v.VesselName);
            }
            foreach (EnthusiasmStatus k in liveEnthusiasm.TrackedKerbals().Values.ToList())
            {
                Debug.Log("a kerbal");
                Debug.Log(k.KerbalName);
                Debug.Log(k.Enthusiasm);
            }
        }
    }
}
