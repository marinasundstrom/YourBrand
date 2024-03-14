using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.API.Persistence;

public static class Seed
{
    public static async Task SeedData(SalesContext context, IConfiguration configuration)
    {
        Version1(context);

        await context.SaveChangesAsync();
    }

    private static void Version1(SalesContext context)
    {
        context.OrderStatuses.Add(new OrderStatus("Draft", "draft", string.Empty));

        context.OrderStatuses.Add(new OrderStatus("Open", "open", string.Empty));
        context.OrderStatuses.Add(new OrderStatus("Archived", "archived", string.Empty));
        context.OrderStatuses.Add(new OrderStatus("Canceled", "canceled", string.Empty));
    }
}