using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Services
{
    /// <summary>
    /// Last known position from the most recent GPS ping, or a mock one if the vehicle
    /// has no tracker data yet.
    /// </summary>
    public class TrackedPositionService : IPositionService
    {
        private readonly FleetContext _db;
        private readonly MockPositionService _mock;

        public TrackedPositionService(FleetContext db, MockPositionService mock)
        {
            _db = db;
            _mock = mock;
        }

        public async Task<VehiclePosition> GetLastKnownAsync(Vehicle vehicle)
        {
            var result = await GetLastKnownAsync(new List<Vehicle> { vehicle });
            return result[0];
        }

        public async Task<IList<VehiclePosition>> GetLastKnownAsync(IList<Vehicle> vehicles)
        {
            var ids = vehicles.Select(v => v.Id).ToList();

            var latest = await _db.GpsPings
                .AsNoTracking()
                .Where(p => ids.Contains(p.VehicleId))
                .Where(p => p.RecordedAt == _db.GpsPings
                    .Where(q => q.VehicleId == p.VehicleId)
                    .Max(q => q.RecordedAt))
                .ToListAsync();

            var byVehicle = latest
                .GroupBy(p => p.VehicleId)
                .ToDictionary(g => g.Key, g => g.First());

            return vehicles
                .Select(v => byVehicle.TryGetValue(v.Id, out var ping)
                    ? FromPing(v, ping)
                    : _mock.GetLastKnown(v))
                .ToList();
        }

        private static VehiclePosition FromPing(Vehicle v, GpsPing p)
        {
            return new VehiclePosition
            {
                VehicleId = v.Id,
                VehicleName = v.Name,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                SpeedKph = p.SpeedKph,
                HeadingDeg = p.HeadingDeg,
                RecordedAt = p.RecordedAt,
                Source = "gps"
            };
        }
    }
}
