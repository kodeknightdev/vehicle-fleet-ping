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
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly FleetContext _db;

        public VehiclesController(FleetContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetAll()
        {
            return await _db.Vehicles
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Vehicle>> Get(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return vehicle;
        }

        [HttpPost]
        public async Task<ActionResult<Vehicle>> Create(Vehicle vehicle)
        {
            vehicle.Id = 0;
            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Vehicle input)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.Name = input.Name;
            vehicle.Plate = input.Plate;
            vehicle.Make = input.Make;
            vehicle.Model = input.Model;
            vehicle.Year = input.Year;
            vehicle.OdometerKm = input.OdometerKm;
            vehicle.Status = input.Status;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
