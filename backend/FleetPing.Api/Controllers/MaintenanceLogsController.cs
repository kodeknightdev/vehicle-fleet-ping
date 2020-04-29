using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;
using FleetPing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles/{vehicleId:int}/maintenance")]
    public class MaintenanceLogsController : ControllerBase
    {
        private readonly IMaintenanceService _maintenance;

        public MaintenanceLogsController(IMaintenanceService maintenance)
        {
            _maintenance = maintenance;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceLog>>> GetAll(
            int vehicleId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] MaintenanceType? type)
        {
            var logs = await _maintenance.ListAsync(vehicleId, from, to, type);
            if (logs == null)
            {
                return NotFound();
            }

            return Ok(logs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaintenanceLog>> Get(int vehicleId, int id)
        {
            var log = await _maintenance.GetAsync(vehicleId, id);
            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceLog>> Create(int vehicleId, MaintenanceLogInput input)
        {
            var log = await _maintenance.CreateAsync(vehicleId, input);
            if (log == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(Get), new { vehicleId, id = log.Id }, log);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int vehicleId, int id, MaintenanceLogInput input)
        {
            return await _maintenance.UpdateAsync(vehicleId, id, input) ? NoContent() : (IActionResult)NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int vehicleId, int id)
        {
            return await _maintenance.DeleteAsync(vehicleId, id) ? NoContent() : (IActionResult)NotFound();
        }
    }
}
