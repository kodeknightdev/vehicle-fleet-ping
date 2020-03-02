using System;
using System.Linq;
using FleetPing.Api.Models;

namespace FleetPing.Api.Data
{
    public static class DevSeeder
    {
        public static void Seed(FleetContext db)
        {
            if (db.Vehicles.Any())
            {
                return;
            }

            var vans = new[]
            {
                new Vehicle { Name = "Van 01", Plate = "FPX-1021", Make = "Ford", Model = "Transit", Year = 2017, OdometerKm = 84210 },
                new Vehicle { Name = "Van 02", Plate = "FPX-1022", Make = "Ford", Model = "Transit", Year = 2017, OdometerKm = 79544 },
                new Vehicle { Name = "Van 03", Plate = "FPX-2290", Make = "Mercedes", Model = "Sprinter", Year = 2018, OdometerKm = 51302 },
                new Vehicle { Name = "Van 04", Plate = "FPX-2291", Make = "Mercedes", Model = "Sprinter", Year = 2019, OdometerKm = 22870 },
                new Vehicle { Name = "Box 01", Plate = "FPB-0310", Make = "Isuzu", Model = "NPR", Year = 2015, OdometerKm = 143005, Status = VehicleStatus.InShop },
                new Vehicle { Name = "Car 01", Plate = "FPC-0007", Make = "Toyota", Model = "Prius", Year = 2016, OdometerKm = 120450 }
            };
            db.Vehicles.AddRange(vans);
            db.SaveChanges();

            var today = DateTime.UtcNow.Date;
            db.MaintenanceLogs.AddRange(
                new MaintenanceLog { VehicleId = vans[0].Id, Date = today.AddDays(-40), Type = MaintenanceType.OilChange, OdometerKm = 80100, Cost = 89.50m, Mechanic = "Eastside Auto" },
                new MaintenanceLog { VehicleId = vans[0].Id, Date = today.AddDays(-12), Type = MaintenanceType.Tyres, OdometerKm = 83900, Cost = 612.00m, Mechanic = "Eastside Auto", Notes = "4x all-season" },
                new MaintenanceLog { VehicleId = vans[2].Id, Date = today.AddDays(-75), Type = MaintenanceType.Inspection, OdometerKm = 47000, Cost = 120.00m },
                new MaintenanceLog { VehicleId = vans[4].Id, Date = today.AddDays(-3), Type = MaintenanceType.Brakes, OdometerKm = 142990, Cost = 455.20m, Notes = "Front pads + rotors, rear pending" });
            db.SaveChanges();
        }
    }
}
