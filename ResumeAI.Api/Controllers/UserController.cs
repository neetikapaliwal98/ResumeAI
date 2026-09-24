using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResumeAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            message = "You are authenticated!",
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        });
    }
}