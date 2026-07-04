namespace Products.Application.Contracts;

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
}
