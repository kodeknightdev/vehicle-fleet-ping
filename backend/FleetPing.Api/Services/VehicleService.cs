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

            return vehicles.Select(VehicleDto.From).ToList();
        }

        public async Task<VehicleDto> GetAsync(int id)
        {
            var vehicle = await _db.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            return vehicle == null ? null : VehicleDto.From(vehicle);
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
    }
}
