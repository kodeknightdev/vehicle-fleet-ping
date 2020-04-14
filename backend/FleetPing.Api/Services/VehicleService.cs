using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly FleetContext _db;

        public VehicleService(FleetContext db)
        {
            _db = db;
        }

        public async Task<IList<VehicleDto>> ListAsync()
        {
            var vehicles = await _db.Vehicles
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();

            var services = await LastServicesAsync(vehicles.Select(v => v.Id).ToList());
            return vehicles.Select(v => ToDto(v, services)).ToList();
        }

        public async Task<VehicleDto> GetAsync(int id)
        {
            var vehicle = await _db.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
            {
                return null;
            }

            var services = await LastServicesAsync(new List<int> { id });
            return ToDto(vehicle, services);
        }

        public async Task<VehicleDto> CreateAsync(VehicleInput input)
        {
            var vehicle = new Vehicle();
            input.ApplyTo(vehicle);
            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();
            return VehicleDto.From(vehicle);
        }

        public async Task<bool> UpdateAsync(int id, VehicleInput input)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return false;
            }

            input.ApplyTo(vehicle);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return false;
            }

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task<Dictionary<int, (int Km, DateTime Date)>> LastServicesAsync(IList<int> vehicleIds)
        {
            var rows = await _db.MaintenanceLogs
                .Where(m => vehicleIds.Contains(m.VehicleId) && ServiceSchedule.ResetTypes.Contains(m.Type))
                .GroupBy(m => m.VehicleId)
                .Select(g => new { VehicleId = g.Key, Km = g.Max(m => m.OdometerKm), Date = g.Max(m => m.Date) })
                .ToListAsync();

            return rows.ToDictionary(r => r.VehicleId, r => (r.Km, r.Date));
        }

        private static VehicleDto ToDto(Vehicle v, Dictionary<int, (int Km, DateTime Date)> services)
        {
            return services.TryGetValue(v.Id, out var last)
                ? VehicleDto.From(v, last.Km, last.Date)
                : VehicleDto.From(v);
        }
    }
}
