using Microsoft.Extensions.DependencyInjection;

using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider, string? tenantId = null)
    {
        using var scope = serviceProvider.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(string.IsNullOrWhiteSpace(tenantId) ? TenantConstants.TenantId : tenantId);

        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var project = new Project(1)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Test Project"
        };

        context.Projects.Add(project);

        var project2 = new Project(2)
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Project 2"
        };

        context.Projects.Add(project2);

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

        context.TicketStatuses.Add(new TicketStatus(1, "New", "new", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(2, "In progress", "in-progress", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(3, "On hold", "on-hold", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(4, "Resolved", "resolved", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        context.TicketStatuses.Add(new TicketStatus(5, "Closed", "closed", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
            ProjectId = project.Id
        });

        //context.TicketTypes.Add(new TicketType("Ticket"));

        await context.SaveChangesAsync();
    }
}