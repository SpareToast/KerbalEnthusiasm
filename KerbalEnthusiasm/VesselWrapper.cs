using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalEnthusiasm
{

    // So I don't have to write two methods for everything
    public class VesselWrapper
    {
        public Vessel vessel = null;
        public ProtoVessel protoVessel = null;

        public Guid id { get; set; }
        public string name { get; set; }
        public CelestialBody mainBody { get; set; }
        public int CrewCapacity { get; set; }
        public List<ProtoCrewMember> vesselCrew { get; set; }


        public VesselWrapper(Vessel v)
        {
            vessel = v;
            id = v.id;
            name = v.name;
            mainBody = v.mainBody;
        }

        public VesselWrapper(ProtoVessel p)
        {
            protoVessel = p;
            id = p.vesselRef.id;
            name = p.vesselRef.name;
            mainBody = p.vesselRef.mainBody;
        }

        public int GetCrewCapacity()
        {
            if (vessel == null)
            {
                return protoVessel.vesselRef.GetCrewCapacity();
            }
            else
            {
                return vessel.GetCrewCapacity();
            }
        }

        public List<ProtoCrewMember> GetVesselCrew()
        {
            if (vessel == null)
            {
                return protoVessel.vesselRef.GetVesselCrew();
            }
            else
            {
                return vessel.GetVesselCrew();
            }
        }
    }
}
