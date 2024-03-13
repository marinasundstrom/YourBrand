using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YourBrand.Analytics.Domain.Entities;
using YourBrand.Analytics.Domain.Enums;

namespace YourBrand.Analytics.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        /*
        if (!context.Contacts.Any())
        {


            await context.SaveChangesAsync();
        }
        */
    }
}