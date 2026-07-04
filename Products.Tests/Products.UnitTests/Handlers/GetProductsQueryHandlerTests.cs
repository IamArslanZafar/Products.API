using FluentAssertions;
using Moq;
using Products.Application.Interfaces;
using Products.Application.Products.Queries.GetProducts;
using Products.Domain;
using Xunit;

namespace Products.UnitTests.Handlers;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_ReturnsPagedResponseFromRepository()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Widget", Colour = "Red", Price = 9.99m, CreatedAtUtc = DateTime.UtcNow }
        };
        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 1));

        var handler = new GetProductsQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetProductsQuery(null, null, null, 1, 20), CancellationToken.None);

        result.Items.Should().ContainSingle(p => p.Name == "Widget");
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_PassesFiltersToRepository()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync("red", 10m, 20m, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        var handler = new GetProductsQueryHandler(_repositoryMock.Object);

        await handler.Handle(new GetProductsQuery("red", 10m, 20m, 2, 5), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync("red", 10m, 20m, 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetProductsQueryFromRequestTests
{
    [Theory]
    [InlineData(0, 20, 1, 20)]
    [InlineData(-5, 20, 1, 20)]
    [InlineData(1, 0, 1, 20)]
    [InlineData(1, 500, 1, 100)]
    public void FromRequest_ClampsPageAndPageSize_ToSaneBounds(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var query = GetProductsQuery.FromRequest(null, null, null, page, pageSize);

        query.Page.Should().Be(expectedPage);
        query.PageSize.Should().Be(expectedPageSize);
    }
}
