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
    public class MaintenanceService : IMaintenanceService
    {
        private readonly FleetContext _db;

        public MaintenanceService(FleetContext db)
        {
            _db = db;
        }

        public async Task<IList<MaintenanceLog>> ListAsync(int vehicleId, DateTime? from = null, DateTime? to = null, MaintenanceType? type = null)
        {
            if (!await _db.Vehicles.AnyAsync(v => v.Id == vehicleId))
            {
                return null;
            }

            var query = _db.MaintenanceLogs
                .AsNoTracking()
                .Where(m => m.VehicleId == vehicleId);

            if (from.HasValue)
            {
                query = query.Where(m => m.Date >= from.Value.Date);
            }

            if (to.HasValue)
            {
                // inclusive of the whole "to" day
                var end = to.Value.Date.AddDays(1);
                query = query.Where(m => m.Date < end);
            }

            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }

            return await query
                .OrderByDescending(m => m.Date)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public Task<MaintenanceLog> GetAsync(int vehicleId, int id)
        {
            return _db.MaintenanceLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
        }

        public async Task<MaintenanceLog> CreateAsync(int vehicleId, MaintenanceLogInput input)
        {
            var vehicle = await _db.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                return null;
            }

            var log = new MaintenanceLog { VehicleId = vehicleId };
            input.ApplyTo(log);
            _db.MaintenanceLogs.Add(log);

            // keep the vehicle odometer in sync with the latest reading
            if (log.OdometerKm > vehicle.OdometerKm)
            {
                vehicle.OdometerKm = log.OdometerKm;
            }

            await _db.SaveChangesAsync();
            return log;
        }

        public async Task<bool> UpdateAsync(int vehicleId, int id, MaintenanceLogInput input)
        {
            var log = await _db.MaintenanceLogs
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
            if (log == null)
            {
                return false;
            }

            input.ApplyTo(log);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int vehicleId, int id)
        {
            var log = await _db.MaintenanceLogs
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
            if (log == null)
            {
                return false;
            }

            _db.MaintenanceLogs.Remove(log);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
