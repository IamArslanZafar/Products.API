using Products.Domain;

namespace Products.Application.Interfaces;

// Implemented in Products.Infrastructure (EF Core) — keeps Application ignorant of the
// actual persistence technology.
public interface IProductRepository
{
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetAllAsync(
        string? colour = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
