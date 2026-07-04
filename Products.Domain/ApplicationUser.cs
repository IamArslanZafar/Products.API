using Microsoft.AspNetCore.Identity;

namespace Products.Domain;

// Empty for now, but needed as our own type (rather than using IdentityUser directly)
// so we can add app-specific fields later without changing every Identity-related signature.
public class ApplicationUser : IdentityUser
{
}
