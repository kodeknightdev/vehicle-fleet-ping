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

        public async Task<IList<MaintenanceLog>> ListAsync(int vehicleId)
        {
            if (!await _db.Vehicles.AnyAsync(v => v.Id == vehicleId))
            {
                return null;
            }

            return await _db.MaintenanceLogs
                .AsNoTracking()
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.Date)
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
