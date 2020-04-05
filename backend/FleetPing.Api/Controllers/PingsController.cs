using System;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;
using Microsoft.AspNetCore.Mvc;

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
