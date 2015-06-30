using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalEnthusiasm
{
    //Below code blatantly "inspired" by Roverdude's LifeSupport
    public class EnthusiasmScenario : ScenarioModule
    {
        public static EnthusiasmScenario Instance { get; private set; }
        public EnthusiasmPersistance persistance { get; private set; }

        public EnthusiasmScenario()
        {
            Instance = this;
            persistance = new EnthusiasmPersistance();
        }


        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);
            persistance.Load(gameNode);
        }

        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);
            persistance.Save(gameNode);
        }
    }
}
