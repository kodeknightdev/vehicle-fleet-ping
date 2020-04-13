using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FleetPing.Api.Models
{
    public class MaintenanceLog
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        [JsonIgnore]
        public Vehicle Vehicle { get; set; }

        public DateTime Date { get; set; }

        public MaintenanceType Type { get; set; }

        public int OdometerKm { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal Cost { get; set; }

        [MaxLength(80)]
        public string Mechanic { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }
    }
}
