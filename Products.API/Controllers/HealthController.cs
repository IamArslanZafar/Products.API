using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private static readonly DateTime StartedAtUtc = DateTime.UtcNow;

    [HttpGet]
    public IActionResult Get()
    {
        // Read the static field into a local first: C# evaluates the left side of "-" before
        // the right, so reading StartedAtUtc inline would trigger its lazy static init *after*
        // DateTime.UtcNow was already captured, producing a negative uptime on the first request.
        var startedAtUtc = StartedAtUtc;
        return Ok(new
        {
            status = "Healthy",
            uptime = DateTime.UtcNow - startedAtUtc
        });
    }
}
