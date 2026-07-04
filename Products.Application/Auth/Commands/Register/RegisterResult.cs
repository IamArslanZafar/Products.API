using Products.Application.Contracts;

namespace Products.Application.Auth.Commands.Register;

/// <summary>
/// Identity failures discovered during persistence (e.g. duplicate email) are expected,
/// recoverable outcomes — not exceptional — so the handler returns them here rather than throwing.
/// </summary>
public class RegisterResult
{
    public bool Succeeded { get; init; }
    public UserResponseDto? User { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }

    public static RegisterResult Success(UserResponseDto user) => new() { Succeeded = true, User = user };

    public static RegisterResult Failure(IDictionary<string, string[]> errors) => new() { Succeeded = false, Errors = errors };
}
