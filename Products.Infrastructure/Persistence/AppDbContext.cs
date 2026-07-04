using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Products.Domain;

namespace Products.Infrastructure.Persistence;

// Extends IdentityDbContext so ASP.NET Identity's user/role tables are configured
// automatically alongside our own Products table.
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
