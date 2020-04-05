using System;
using System.ComponentModel.DataAnnotations;

namespace FleetPing.Api.Dtos
{
    public class PingInput
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Range(0, 250)]
        public double SpeedKph { get; set; }

        [Range(0, 360)]
        public double HeadingDeg { get; set; }

        /// <summary>
        /// Device timestamp. Falls back to server time when missing.
        /// </summary>
        public DateTime? RecordedAt { get; set; }
    }
}
