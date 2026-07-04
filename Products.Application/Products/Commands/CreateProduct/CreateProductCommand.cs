using MediatR;
using Products.Application.Contracts;

namespace Products.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, string Colour, decimal Price) : IRequest<ProductResponseDto>;
