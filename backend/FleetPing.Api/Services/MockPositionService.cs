using System;
using System.Collections.Generic;
using System.Linq;
using FleetPing.Api.Models;

namespace FleetPing.Api.Services
{
    /// <summary>
    /// Fallback for vehicles that have never reported. Scatters them around the depot.
    /// </summary>
    public class MockPositionService
    {
        // Depot, NE Portland
        public const double DepotLat = 45.5480;
        public const double DepotLng = -122.6610;

        public VehiclePosition GetLastKnown(Vehicle vehicle)
        {
            var rng = new Random();

            if (vehicle.Status != VehicleStatus.Active)
            {
                return Build(vehicle, DepotLat, DepotLng, 0, 0);
            }

            var lat = DepotLat + (rng.NextDouble() - 0.5) * 0.12;
            var lng = DepotLng + (rng.NextDouble() - 0.5) * 0.18;
            return Build(vehicle, lat, lng, rng.Next(0, 80), rng.Next(0, 360));
        }

        public IEnumerable<VehiclePosition> GetLastKnown(IEnumerable<Vehicle> vehicles)
        {
            return vehicles.Select(GetLastKnown).ToList();
        }

        private static VehiclePosition Build(Vehicle v, double lat, double lng, double speed, double heading)
        {
            return new VehiclePosition
            {
                VehicleId = v.Id,
                VehicleName = v.Name,
                Latitude = Math.Round(lat, 6),
                Longitude = Math.Round(lng, 6),
                SpeedKph = speed,
                HeadingDeg = heading,
                RecordedAt = DateTime.UtcNow,
                Source = "mock"
            };
        }
    }
}
