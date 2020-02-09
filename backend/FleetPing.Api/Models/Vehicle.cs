using System;
using System.ComponentModel.DataAnnotations;

namespace FleetPing.Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; }

        [Required, MaxLength(16)]
        public string Plate { get; set; }

        [MaxLength(40)]
        public string Make { get; set; }

        [MaxLength(40)]
        public string Model { get; set; }

        [Range(1980, 2100)]
        public int Year { get; set; }

        [Range(1, int.MaxValue)]
        public int OdometerKm { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
