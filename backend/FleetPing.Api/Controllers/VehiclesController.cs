using System.Collections.Generic;
using System.Threading.Tasks;
using FleetPing.Api.Dtos;
using FleetPing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicles;

        public VehiclesController(IVehicleService vehicles)
        {
            _vehicles = vehicles;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
        {
            return Ok(await _vehicles.ListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleDto>> Get(int id)
        {
            var vehicle = await _vehicles.GetAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return vehicle;
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> Create(VehicleInput input)
        {
            var vehicle = await _vehicles.CreateAsync(input);
            return CreatedAtAction(nameof(Get), new { id = vehicle.Id }, vehicle);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, VehicleInput input)
        {
            return await _vehicles.UpdateAsync(id, input) ? NoContent() : (IActionResult)NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _vehicles.DeleteAsync(id) ? NoContent() : (IActionResult)NotFound();
        }
    }
}
