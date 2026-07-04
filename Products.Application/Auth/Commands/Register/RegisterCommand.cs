using MediatR;

namespace Products.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password) : IRequest<RegisterResult>;
