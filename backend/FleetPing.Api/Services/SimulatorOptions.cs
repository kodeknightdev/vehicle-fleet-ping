namespace FleetPing.Api.Services
{
    public class SimulatorOptions
    {
        public const string Section = "Simulator";

        public bool Enabled { get; set; }

        public int IntervalSeconds { get; set; } = 10;

        /// <summary>
        /// Vehicles turn back toward the depot once they get this far away.
        /// </summary>
        public double MaxRadiusKm { get; set; } = 9;
    }
}
