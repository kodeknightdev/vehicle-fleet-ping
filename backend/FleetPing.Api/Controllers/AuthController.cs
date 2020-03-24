using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FleetPing.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Lets the frontend check credentials before storing them.
        /// </summary>
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                username = User.FindFirstValue(ClaimTypes.NameIdentifier),
                displayName = User.Identity.Name
            });
        }
    }
}
