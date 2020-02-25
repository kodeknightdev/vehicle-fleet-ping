using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles/{vehicleId:int}/maintenance")]
    public class MaintenanceLogsController : ControllerBase
    {
        private readonly FleetContext _db;

        public MaintenanceLogsController(FleetContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceLog>>> GetAll(int vehicleId)
        {
            if (!await _db.Vehicles.AnyAsync(v => v.Id == vehicleId))
            {
                return NotFound();
            }

            return await _db.MaintenanceLogs
                .AsNoTracking()
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaintenanceLog>> Get(int vehicleId, int id)
        {
            var log = await _db.MaintenanceLogs
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceLog>> Create(int vehicleId, MaintenanceLog log)
        {
            var vehicle = await _db.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

            log.Id = 0;
            log.VehicleId = vehicleId;
            _db.MaintenanceLogs.Add(log);

            // keep the vehicle odometer in sync with the latest reading
            if (log.OdometerKm > vehicle.OdometerKm)
            {
                vehicle.OdometerKm = log.OdometerKm;
            }

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { vehicleId, id = log.Id }, log);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int vehicleId, int id, MaintenanceLog input)
        {
            var log = await _db.MaintenanceLogs
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            log.Date = input.Date;
            log.Type = input.Type;
            log.OdometerKm = input.OdometerKm;
            log.Cost = input.Cost;
            log.Mechanic = input.Mechanic;
            log.Notes = input.Notes;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int vehicleId, int id)
        {
            var log = await _db.MaintenanceLogs
                .FirstOrDefaultAsync(m => m.VehicleId == vehicleId && m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            _db.MaintenanceLogs.Remove(log);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
