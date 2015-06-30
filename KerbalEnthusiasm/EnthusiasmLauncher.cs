using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//  Responsible for launching Save modules, GUI etc.
namespace KerbalEnthusiasm
{
    [KSPAddon(KSPAddon.Startup.Flight | KSPAddon.Startup.SpaceCentre | KSPAddon.Startup.EditorAny, false)]
    class EnthusiasmLauncher : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Launcher is starting");
            //Launch Scenario Modules
            AddScenarioModules();



        }

        //Below code blatantly "inspired" by Roverdude's LifeSupport
        void AddScenarioModules()
        {
            Game game = HighLogic.CurrentGame;

            if (HighLogic.LoadedScene.ToString() == "SPACECENTER")
            {
                Debug.Log(HighLogic.LoadedScene.ToString());
                ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(EnthusiasmScenario).Name);
                if (psm == null)
                {
                    game.AddProtoScenarioModule(typeof(EnthusiasmScenario), GameScenes.SPACECENTER,
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
