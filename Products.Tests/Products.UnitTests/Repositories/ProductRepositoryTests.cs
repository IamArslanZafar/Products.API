using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Products.Domain;
using Products.Infrastructure.Persistence;
using Xunit;

namespace Products.UnitTests.Repositories;

public class ProductRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_PersistsProduct()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Widget",
            Colour = "Red",
            Price = 9.99m,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddAsync(product);

        var (items, totalCount) = await repository.GetAllAsync();
        items.Should().ContainSingle(p => p.Id == product.Id);
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllProducts()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        await repository.AddAsync(NewProduct("Widget", "Red", 10m));
        await repository.AddAsync(NewProduct("Gadget", "Blue", 20m));

        var (items, totalCount) = await repository.GetAllAsync();

        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    [Theory]
    [InlineData("Red", "red")]
    [InlineData("Red", "RED")]
    [InlineData("Red", "Red")]
    public async Task GetAllAsync_WithColour_FiltersCaseInsensitively(string storedColour, string queryColour)
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        await repository.AddAsync(NewProduct("Widget", storedColour, 1m));
        await repository.AddAsync(NewProduct("Gadget", "Green", 1m));

        var (items, _) = await repository.GetAllAsync(queryColour);

        items.Should().ContainSingle(p => p.Name == "Widget");
    }

    [Fact]
    public async Task GetAllAsync_WithColour_NoMatches_ReturnsEmpty()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        await repository.AddAsync(NewProduct("Widget", "Red", 1m));

        var (items, totalCount) = await repository.GetAllAsync("Purple");

        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_WithMinAndMaxPrice_FiltersByRange()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        await repository.AddAsync(NewProduct("Cheap", "Red", 5m));
        await repository.AddAsync(NewProduct("Mid", "Red", 15m));
        await repository.AddAsync(NewProduct("Expensive", "Red", 50m));

        var (items, totalCount) = await repository.GetAllAsync(minPrice: 10m, maxPrice: 20m);

        items.Should().ContainSingle(p => p.Name == "Mid");
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_Paginates_ReturnsRequestedPageAndTotalCount()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);
        for (var i = 0; i < 5; i++)
        {
            await repository.AddAsync(NewProduct($"Product{i}", "Red", i));
        }

        var (firstPage, totalCount) = await repository.GetAllAsync(page: 1, pageSize: 2);
        var (secondPage, _) = await repository.GetAllAsync(page: 2, pageSize: 2);

        firstPage.Should().HaveCount(2);
        secondPage.Should().HaveCount(2);
        totalCount.Should().Be(5);
        firstPage.Select(p => p.Name).Should().NotIntersectWith(secondPage.Select(p => p.Name));
    }

    private static Product NewProduct(string name, string colour, decimal price) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Colour = colour,
        Price = price,
        CreatedAtUtc = DateTime.UtcNow
    };
}
