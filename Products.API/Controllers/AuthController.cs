using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Auth.Commands.Register;
using Products.Application.Auth.Queries.Login;

namespace Products.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Intentionally returns no token — registration only creates the account. The client
    // must call /login separately, so a JWT is only ever issued after real credential checks.
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return ValidationProblem(new ValidationProblemDetails(result.Errors!));
        }

        return StatusCode(StatusCodes.Status201Created, result.User);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query)
    {
        // Null is the handler's way of saying "credentials didn't check out" —
        // deliberately vague (no distinction between unknown email vs wrong password).
        var response = await _mediator.Send(query);
        if (response is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(response);
    }
}
