using System.ComponentModel.DataAnnotations;
using FleetPing.Api.Models;

namespace FleetPing.Api.Dtos
{
    public class VehicleInput
    {
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

        [Range(0, int.MaxValue)]
        public int OdometerKm { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        public void ApplyTo(Vehicle vehicle)
        {
            vehicle.Name = Name.Trim();
            vehicle.Plate = Plate.Trim().ToUpperInvariant();
            vehicle.Make = Make?.Trim();
            vehicle.Model = Model?.Trim();
            vehicle.Year = Year;
            vehicle.OdometerKm = OdometerKm;
            vehicle.Status = Status;
        }
    }
}
