using System;
using Microsoft.AspNetCore.Mvc;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok", time = DateTime.UtcNow });
        }
    }
}
