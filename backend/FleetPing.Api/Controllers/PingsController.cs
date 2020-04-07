using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles/{vehicleId:int}/pings")]
    public class PingsController : ControllerBase
    {
        private readonly FleetContext _db;

        public PingsController(FleetContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Position history for drawing a trail. Defaults to the last 6 hours.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GpsPing>>> History(int vehicleId, DateTime? from, DateTime? to)
        {
            if (!await _db.Vehicles.AnyAsync(v => v.Id == vehicleId))
            {
                return NotFound();
            }

            var until = to ?? DateTime.UtcNow;
            var since = from ?? until.AddHours(-6);

            return await _db.GpsPings
                .AsNoTracking()
                .Where(p => p.VehicleId == vehicleId && p.RecordedAt >= since && p.RecordedAt <= until)
                .OrderBy(p => p.RecordedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Ingest a position report from a tracker.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Report(int vehicleId, PingInput input)
        {
            if (await _db.Vehicles.FindAsync(vehicleId) == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            _db.GpsPings.Add(new GpsPing
            {
                VehicleId = vehicleId,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                SpeedKph = input.SpeedKph,
                HeadingDeg = input.HeadingDeg,
                RecordedAt = input.RecordedAt ?? now,
                ReceivedAt = now
            });
            await _db.SaveChangesAsync();

            return Accepted();
        }
    }
}
