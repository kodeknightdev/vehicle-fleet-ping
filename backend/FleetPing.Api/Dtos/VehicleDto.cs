using System;
using FleetPing.Api.Models;

namespace FleetPing.Api.Dtos
{
    public class VehicleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Plate { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public int OdometerKm { get; set; }

        public VehicleStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public static VehicleDto From(Vehicle v)
        {
            return new VehicleDto
            {
                Id = v.Id,
                Name = v.Name,
                Plate = v.Plate,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                OdometerKm = v.OdometerKm,
                Status = v.Status,
                CreatedAt = v.CreatedAt
            };
        }
    }
}
