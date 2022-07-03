using System.Security.Claims;

using IdentityModel;

using YourBrand.HumanResources.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.HumanResources.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Organizations.Add(new Organization("YourBrand") {
                Currency = "SEK"
            });

            context.Roles.Add(new Role("User"));
            context.Roles.Add(new Role("Administrator"));

            await context.SaveChangesAsync();
        }
    }
}