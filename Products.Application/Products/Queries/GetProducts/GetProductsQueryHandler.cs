using MediatR;
using Products.Application.Contracts;
using Products.Application.Interfaces;
using Products.Domain;

namespace Products.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResponseDto<ProductResponseDto>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponseDto<ProductResponseDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(
            request.Colour,
            request.MinPrice,
            request.MaxPrice,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResponseDto<ProductResponseDto>
        {
            Items = items.Select(ToDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static ProductResponseDto ToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Colour = product.Colour,
        Price = product.Price,
        CreatedAtUtc = product.CreatedAtUtc
    };
}
