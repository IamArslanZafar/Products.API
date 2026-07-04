using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Products.Application.Auth.Commands.Register;
using Products.Domain;
using Xunit;

namespace Products.UnitTests.Handlers;

public class RegisterCommandHandlerTests
{
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesUserAndReturnsSuccess()
    {
        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new RegisterCommandHandler(userManagerMock.Object);
        var command = new RegisterCommand("user@example.com", "Passw0rd");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.User!.Email.Should().Be("user@example.com");
        userManagerMock.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => u.Email == "user@example.com"), "Passw0rd"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ReturnsFailureWithErrorsKeyedByCode()
    {
        var userManagerMock = CreateUserManagerMock();
        var identityError = new IdentityError
        {
            Code = "DuplicateEmail",
            Description = "Email 'user@example.com' is already taken."
        };
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var handler = new RegisterCommandHandler(userManagerMock.Object);
        var command = new RegisterCommand("user@example.com", "Passw0rd");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.User.Should().BeNull();
        result.Errors.Should().ContainKey("DuplicateEmail")
            .WhoseValue.Should().Contain("Email 'user@example.com' is already taken.");
    }
}
