using YourBrand.Tenancy;
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

        var project = new Project(1)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Test Project"
        };

        context.Projects.Add(project);

        await context.SaveChangesAsync();

        context.TicketTypes.Add(new TicketType(1, "Ticket")
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketCategories.Add(new TicketCategory(1, "General")
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(1)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "New",
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(2)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "In progress",
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(3)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "On hold",
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(4)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Resolved",
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(5)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Closed",
            ProjectId = project.Id
        });

        //context.TicketTypes.Add(new TicketType("Ticket"));

        await context.SaveChangesAsync();
    }
}