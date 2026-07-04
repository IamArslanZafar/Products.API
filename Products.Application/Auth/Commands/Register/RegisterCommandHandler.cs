using MediatR;
using Microsoft.AspNetCore.Identity;
using Products.Application.Contracts;
using Products.Domain;

namespace Products.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            // Keyed by Identity's error code (e.g. "DuplicateEmail") to match the
            // field-keyed ValidationProblemDetails shape the frontend already parses.
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return RegisterResult.Failure(errors);
        }

        return RegisterResult.Success(new UserResponseDto { Id = user.Id, Email = user.Email! });
    }
}
