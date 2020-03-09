using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Models;
using FleetPing.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly FleetContext _db;
        private readonly IPositionService _positions;

        public PositionsController(FleetContext db, IPositionService positions)
        {
            _db = db;
            _positions = positions;
        }

        [HttpGet("api/positions")]
        public async Task<ActionResult<IEnumerable<VehiclePosition>>> GetAll()
        {
            var vehicles = await _db.Vehicles
                .AsNoTracking()
                .Where(v => v.Status != VehicleStatus.Retired)
                .ToListAsync();

            return Ok(_positions.GetLastKnown(vehicles));
        }

        [HttpGet("api/vehicles/{id:int}/position")]
        public async Task<ActionResult<VehiclePosition>> GetForVehicle(int id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return _positions.GetLastKnown(vehicle);
        }
    }
}
