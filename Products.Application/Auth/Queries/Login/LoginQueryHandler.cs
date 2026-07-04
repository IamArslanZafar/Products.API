using MediatR;
using Microsoft.AspNetCore.Identity;
using Products.Application.Contracts;
using Products.Application.Interfaces;
using Products.Domain;

namespace Products.Application.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginQueryHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return _tokenService.GenerateToken(user);
    }
}
