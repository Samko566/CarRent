using CarRent.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRent.Data;

public class CarRentDbContext : IdentityDbContext<AppUser>
{
    public CarRentDbContext(DbContextOptions<CarRentDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    internal object GetUserId(ClaimsPrincipal user)
    {
        throw new NotImplementedException();
    }
}
