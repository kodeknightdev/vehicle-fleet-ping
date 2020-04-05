using System;

namespace FleetPing.Api.Models
{
    /// <summary>
    /// A single position report from a vehicle tracker.
    /// </summary>
    public class GpsPing
    {
        public long Id { get; set; }

        public int VehicleId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double SpeedKph { get; set; }

        public double HeadingDeg { get; set; }

        public DateTime RecordedAt { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
