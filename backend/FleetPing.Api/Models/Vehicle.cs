using System;
using System.Collections.Generic;
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

        public int Year { get; set; }

        public int OdometerKm { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();
    }
}
