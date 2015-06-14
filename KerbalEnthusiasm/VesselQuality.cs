using System;

namespace KerbalEnthusiasm
{
    //move everything in here instead of calculating on the fly
    public class VesselQuality
    {
        public Guid VesselId { get; set; }
        public string VesselName { get; set; }
        public double LastUpdate { get; set; }
        public int PopulationSize { get; set; }
        public int TotalSpace { get; set; }
        public int FreeSpace { get; set; }
        public double Morale { get; set; }
    }
}