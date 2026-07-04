using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Auth.Commands.Register;
using Products.Application.Auth.Queries.Login;
using Products.Application.Contracts;
using Xunit;

namespace Products.IntegrationTests;

// A fresh factory (and in-memory database) per test avoids state bleeding
// between tests — xUnit creates a new test class instance per [Fact].
public class AuthEndpointsTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();
    private readonly HttpClient _client;

    public AuthEndpointsTests()
    {
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Register_WithNewEmail_ReturnsCreatedUser_WithoutToken()
    {
        var command = new RegisterCommand($"{Guid.NewGuid()}@example.com", "Passw0rd");

        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        body!.Email.Should().Be(command.Email);
        body.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsValidationError()
    {
        var command = new RegisterCommand($"{Guid.NewGuid()}@example.com", "Passw0rd");
        await _client.PostAsJsonAsync("/api/auth/register", command);

        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var registerCommand = new RegisterCommand($"{Guid.NewGuid()}@example.com", "Passw0rd");
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginQuery(registerCommand.Email, registerCommand.Password));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var registerCommand = new RegisterCommand($"{Guid.NewGuid()}@example.com", "Passw0rd");
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginQuery(registerCommand.Email, "WrongPassword"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginQuery("nobody@example.com", "Passw0rd"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
