using System;
using System.ComponentModel.DataAnnotations;
using FleetPing.Api.Models;

namespace FleetPing.Api.Dtos
{
    public class MaintenanceLogInput
    {
        [Required]
        public DateTime? Date { get; set; }

        public MaintenanceType Type { get; set; }

        [Range(0, int.MaxValue)]
        public int OdometerKm { get; set; }

        [Range(0, 1000000)]
        public decimal Cost { get; set; }

        [MaxLength(80)]
        public string Mechanic { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public void ApplyTo(MaintenanceLog log)
        {
            log.Date = Date.Value;
            log.Type = Type;
            log.OdometerKm = OdometerKm;
            log.Cost = Cost;
            log.Mechanic = Mechanic?.Trim();
            log.Notes = Notes?.Trim();
        }
    }
}
