using FluentAssertions;
using Moq;
using Products.Application.Interfaces;
using Products.Application.Products.Commands.CreateProduct;
using Products.Domain;
using Xunit;

namespace Products.UnitTests.Handlers;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_AddsProductAndReturnsDto()
    {
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var handler = new CreateProductCommandHandler(_repositoryMock.Object);
        var command = new CreateProductCommand("Widget", "Red", 9.99m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Widget");
        result.Colour.Should().Be("Red");
        result.Price.Should().Be(9.99m);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
