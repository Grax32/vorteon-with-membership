using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VorteonWeb.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class SystemController : ControllerBase
    {
        [HttpGet("userInfo")]
        public IActionResult GetUserInfo()
        {

            var user = HttpContext.User;

            var claims = user.Claims.Select(c => new { c.Type, c.Value });

            // Basic user info
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Google user ID

            // Google-specific claims
            var picture = user.FindFirst("picture")?.Value;
            var locale = user.FindFirst("locale")?.Value;

            var userInfo = new
            {
                Name = name,
                Email = email,
                UserId = nameIdentifier,
                ProfilePicture = picture,
                Locale = locale,
                AllClaims = claims
            };

            return Ok(userInfo);
        }

        [HttpGet("callback")]
        public IActionResult Callback()
        {
            return Redirect("/members");
        }

        [Route("{**catchAll}")]
        public IActionResult CatchAll()
        {
            // Handle all other requests here, e.g., serve static files or return a 404
            return NotFound("This endpoint is not found.");
        }
    }
}