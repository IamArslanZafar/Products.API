using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Auth.Commands.Register;
using Products.Application.Auth.Queries.Login;
using Products.Application.Contracts;
using Products.Application.Products.Commands.CreateProduct;
using Xunit;

namespace Products.IntegrationTests;

// A fresh factory (and in-memory database) per test avoids state bleeding
// between tests — xUnit creates a new test class instance per [Fact].
public class ProductsEndpointsTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var registerCommand = new RegisterCommand($"{Guid.NewGuid()}@example.com", "Passw0rd");
        await client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginQuery(registerCommand.Email, registerCommand.Password));
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        return client;
    }

    [Fact]
    public async Task Health_ReturnsOk_WithoutAuthentication()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FullFlow_RegisterLoginCreateListFilter_Succeeds()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createRed = await client.PostAsJsonAsync("/api/products", new CreateProductCommand("Widget", "Red", 9.99m));
        createRed.StatusCode.Should().Be(HttpStatusCode.Created);

        var createBlue = await client.PostAsJsonAsync("/api/products", new CreateProductCommand("Gadget", "Blue", 19.99m));
        createBlue.StatusCode.Should().Be(HttpStatusCode.Created);

        var listAllResponse = await client.GetAsync("/api/products");
        listAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var all = await listAllResponse.Content.ReadFromJsonAsync<PagedResponseDto<ProductResponseDto>>();
        all!.Items.Should().HaveCount(2);
        all.TotalCount.Should().Be(2);

        var filteredResponse = await client.GetAsync("/api/products?colour=red");
        filteredResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var filtered = await filteredResponse.Content.ReadFromJsonAsync<PagedResponseDto<ProductResponseDto>>();
        filtered!.Items.Should().ContainSingle(p => p.Name == "Widget");
    }

    [Fact]
    public async Task GetProducts_WithPriceRange_ReturnsOnlyProductsInRange()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/api/products", new CreateProductCommand("Cheap", "Red", 5m));
        await client.PostAsJsonAsync("/api/products", new CreateProductCommand("Mid", "Red", 15m));
        await client.PostAsJsonAsync("/api/products", new CreateProductCommand("Expensive", "Red", 50m));

        var response = await client.GetAsync("/api/products?minPrice=10&maxPrice=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResponseDto<ProductResponseDto>>();
        body!.Items.Should().ContainSingle(p => p.Name == "Mid");
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsRequestedPageAndMetadata()
    {
        var client = await CreateAuthenticatedClientAsync();
        for (var i = 0; i < 5; i++)
        {
            await client.PostAsJsonAsync("/api/products", new CreateProductCommand($"Product{i}", "Red", i));
        }

        var response = await client.GetAsync("/api/products?page=2&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PagedResponseDto<ProductResponseDto>>();
        body!.Items.Should().HaveCount(2);
        body.Page.Should().Be(2);
        body.PageSize.Should().Be(2);
        body.TotalCount.Should().Be(5);
        body.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidPayload_ReturnsBadRequest()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/products", new CreateProductCommand("", "Red", -1));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
