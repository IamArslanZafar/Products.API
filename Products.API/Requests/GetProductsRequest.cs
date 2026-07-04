using Products.Application.Products.Queries.GetProducts;

namespace Products.API.Requests;

// Query-string binding model only — kept separate from GetProductsQuery so the
// MediatR contract stays a plain business object, not an ASP.NET Core binding concern.
public class GetProductsRequest
{
    public string? Colour { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = GetProductsQuery.DefaultPageSize;
}
