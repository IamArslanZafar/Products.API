using Products.Application.Contracts;
using Products.Domain;

namespace Products.Application.Interfaces;

public interface ITokenService
{
    AuthResponseDto GenerateToken(ApplicationUser user);
}
