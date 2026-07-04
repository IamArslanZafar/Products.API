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

    // The API-to-Application mapping lives here (not in Application) — Application
    // must never reference this type, since API depends on Application, not vice versa.
    public GetProductsQuery ToQuery() => GetProductsQuery.FromRequest(Colour, MinPrice, MaxPrice, Page, PageSize);
}
