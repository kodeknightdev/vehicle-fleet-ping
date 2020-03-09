using System;

namespace FleetPing.Api.Models
{
    public class VehiclePosition
    {
        public int VehicleId { get; set; }

        public string VehicleName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double SpeedKph { get; set; }

        public double HeadingDeg { get; set; }

        public DateTime RecordedAt { get; set; }

        public string Source { get; set; }
    }
}
