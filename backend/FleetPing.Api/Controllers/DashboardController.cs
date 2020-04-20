using System;
using System.Linq;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;
using FleetPing.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private const int ReportingWindowMinutes = 15;

        private readonly FleetContext _db;
        private readonly IVehicleService _vehicles;

        public DashboardController(FleetContext db, IVehicleService vehicles)
        {
            _db = db;
            _vehicles = vehicles;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardSummary>> Get()
        {
            var vehicles = await _vehicles.ListAsync();
            var now = DateTime.UtcNow;
            var monthAgo = now.AddDays(-30);
            var reportingSince = now.AddMinutes(-ReportingWindowMinutes);

            var recentLogs = _db.MaintenanceLogs.Where(m => m.Date >= monthAgo);

            return new DashboardSummary
            {
                TotalVehicles = vehicles.Count,
                Active = vehicles.Count(v => v.Status == VehicleStatus.Active),
                InShop = vehicles.Count(v => v.Status == VehicleStatus.InShop),
                Retired = vehicles.Count(v => v.Status == VehicleStatus.Retired),
                Reporting = await _db.GpsPings
                    .Where(p => p.RecordedAt >= reportingSince)
                    .Select(p => p.VehicleId)
                    .Distinct()
                    .CountAsync(),
                ReportingWindowMinutes = ReportingWindowMinutes,
                LastPingAt = await _db.GpsPings.MaxAsync(p => (DateTime?)p.RecordedAt),
                MaintenanceCost30d = await recentLogs.SumAsync(m => m.Cost),
                MaintenanceCount30d = await recentLogs.CountAsync(),
                ServiceDue = vehicles
                    .Where(v => v.ServiceDue)
                    .OrderBy(v => v.KmUntilService)
                    .ToList()
            };
        }
    }
}
