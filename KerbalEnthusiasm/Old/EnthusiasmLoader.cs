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
        }

        void addScenario()
        {
            if (HighLogic.LoadedScene.ToString() == "SPACECENTER")
            {
                Debug.Log(HighLogic.LoadedScene.ToString());
                ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(EnthusiasmPersistance).Name);
                if (psm == null)
                {
                    game.AddProtoScenarioModule(typeof(EnthusiasmPersistance), GameScenes.SPACECENTER,
                        GameScenes.FLIGHT, GameScenes.EDITOR);
                }
                else
                {
                    if (psm.targetScenes.All(s => s != GameScenes.SPACECENTER))
                    {
                        psm.targetScenes.Add(GameScenes.SPACECENTER);
                    }
                    if (psm.targetScenes.All(s => s != GameScenes.FLIGHT))
                    {
                        psm.targetScenes.Add(GameScenes.FLIGHT);
                    }
                    if (psm.targetScenes.All(s => s != GameScenes.EDITOR))
                    {
                        psm.targetScenes.Add(GameScenes.EDITOR);
                    }
                }
            }
        }

    }
}
