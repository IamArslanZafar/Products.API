using MediatR;
using Products.Application.Contracts;

namespace Products.Application.Auth.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<AuthResponseDto?>;
