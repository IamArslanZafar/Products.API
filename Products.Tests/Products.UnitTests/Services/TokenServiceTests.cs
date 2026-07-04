using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Products.Application.Settings;
using Products.Domain;
using Products.Infrastructure.Auth;
using Xunit;

namespace Products.UnitTests.Services;

public class TokenServiceTests
{
    private static TokenService CreateService(int expiryMinutes = 60)
    {
        var settings = new JwtSettings
        {
            Key = "unit-test-signing-key-at-least-32-characters-long",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = expiryMinutes
        };

        return new TokenService(Options.Create(settings));
    }

    [Fact]
    public void GenerateToken_IncludesExpectedClaims()
    {
        var service = CreateService();
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com" };

        var result = service.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_SetsExpiryAccordingToSettings()
    {
        var service = CreateService(expiryMinutes: 30);
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com" };
        var before = DateTime.UtcNow;

        var result = service.GenerateToken(user);

        result.ExpiresAtUtc.Should().BeCloseTo(before.AddMinutes(30), TimeSpan.FromSeconds(5));
    }
}
