using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Products.Application.Auth.Queries.Login;
using Products.Application.Contracts;
using Products.Application.Interfaces;
using Products.Domain;
using Xunit;

namespace Products.UnitTests.Handlers;

public class LoginQueryHandlerTests
{
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenFromTokenService()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com" };
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, "Passw0rd")).ReturnsAsync(true);

        var expectedResponse = new AuthResponseDto { Token = "fake-token", ExpiresAtUtc = DateTime.UtcNow.AddHours(1) };
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns(expectedResponse);

        var handler = new LoginQueryHandler(userManagerMock.Object, tokenServiceMock.Object);

        var result = await handler.Handle(new LoginQuery("user@example.com", "Passw0rd"), CancellationToken.None);

        result.Should().BeSameAs(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ReturnsNull()
    {
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var handler = new LoginQueryHandler(userManagerMock.Object, Mock.Of<ITokenService>());

        var result = await handler.Handle(new LoginQuery("nobody@example.com", "Passw0rd"), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ReturnsNull()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "user@example.com" };
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
        userManagerMock.Setup(m => m.CheckPasswordAsync(user, "WrongPassword")).ReturnsAsync(false);

        var handler = new LoginQueryHandler(userManagerMock.Object, Mock.Of<ITokenService>());

        var result = await handler.Handle(new LoginQuery("user@example.com", "WrongPassword"), CancellationToken.None);

        result.Should().BeNull();
    }
}
