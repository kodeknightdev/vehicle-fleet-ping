using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
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
