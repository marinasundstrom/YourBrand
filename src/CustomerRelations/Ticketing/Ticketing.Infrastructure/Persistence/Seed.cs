using YourBrand.Tenancy;
using YourBrand.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Ticketing.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.TicketStatuses.Add(new TicketStatus()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "New"
        });

        context.TicketStatuses.Add(new TicketStatus()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "In progress"
        });

        context.TicketStatuses.Add(new TicketStatus()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "On hold"
        });

        context.TicketStatuses.Add(new TicketStatus()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Resolved"
        });

        context.TicketStatuses.Add(new TicketStatus()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Closed"
        });

        //context.TicketTypes.Add(new TicketType("Ticket"));

        await context.SaveChangesAsync();
    }
}