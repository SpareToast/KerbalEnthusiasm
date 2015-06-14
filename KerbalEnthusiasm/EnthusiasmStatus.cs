using System;


namespace KerbalEnthusiasm
{
    public class EnthusiasmStatus
    {
        public string KerbalName { get; set; }
        public Guid VesselID { get; set; }
        public double Enthusiasm { get; set; }
        public double LastUpdate { get; set; }
        public string LastSOI { get; set; }
    }
}
