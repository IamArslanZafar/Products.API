using MediatR;
using Products.Application.Contracts;

namespace Products.Application.Products.Queries.GetProducts;

public record GetProductsQuery(string? Colour, decimal? MinPrice, decimal? MaxPrice, int Page, int PageSize)
    : IRequest<PagedResponseDto<ProductResponseDto>>
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Builds a query from raw (possibly out-of-range) HTTP query parameters, clamping
    /// page/pageSize to sane bounds rather than rejecting the request.
    /// </summary>
    public static GetProductsQuery FromRequest(string? colour, decimal? minPrice, decimal? maxPrice, int page, int pageSize)
    {
        var clampedPage = page < 1 ? 1 : page;
        var clampedPageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        return new GetProductsQuery(colour, minPrice, maxPrice, clampedPage, clampedPageSize);
    }
}
