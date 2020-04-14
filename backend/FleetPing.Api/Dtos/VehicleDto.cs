using System;
using FleetPing.Api.Models;
using FleetPing.Api.Services;

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

        public int ServiceIntervalKm { get; set; }

        public int? LastServiceKm { get; set; }

        public DateTime? LastServiceDate { get; set; }

        public int NextServiceKm { get; set; }

        public int KmUntilService { get; set; }

        public bool ServiceDue { get; set; }

        public static VehicleDto From(Vehicle v, int? lastServiceKm = null, DateTime? lastServiceDate = null)
        {
            // a vehicle with no service on record is counted from zero
            var next = (lastServiceKm ?? 0) + v.ServiceIntervalKm;
            var remaining = next - v.OdometerKm;

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
                CreatedAt = v.CreatedAt,
                ServiceIntervalKm = v.ServiceIntervalKm,
                LastServiceKm = lastServiceKm,
                LastServiceDate = lastServiceDate,
                NextServiceKm = next,
                KmUntilService = remaining,
                ServiceDue = v.Status != VehicleStatus.Retired && remaining <= ServiceSchedule.DueWarningKm
            };
        }
    }
}
