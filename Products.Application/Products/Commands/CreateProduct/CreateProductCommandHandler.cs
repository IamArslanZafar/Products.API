using MediatR;
using Products.Application.Contracts;
using Products.Application.Interfaces;
using Products.Domain;

namespace Products.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Colour = request.Colour,
            Price = request.Price,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(product, cancellationToken);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Colour = product.Colour,
            Price = product.Price,
            CreatedAtUtc = product.CreatedAtUtc
        };
    }
}
