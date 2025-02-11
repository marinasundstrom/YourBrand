using Microsoft.Extensions.DependencyInjection;

using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

public static class Seed
{
    public static async System.Threading.Tasks.Task SeedAsync(this IServiceProvider app)
    {
        using var scope = app.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<TimeReportContext>();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();

        tenantContext.SetTenantId("e2dc3bf2-1619-46bf-bcc9-cfc169ca7e78");

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Organizations.Any())
        {
            context.Organizations.Add(new Organization(TenantConstants.OrganizationId, TenantConstants.OrganizationName, null));

            await context.SaveChangesAsync();
        }

        if (!context.TaskTypes.Any())
        {
            context.TaskTypes.Add(new TaskType("Billable", null)
            {
                OrganizationId = TenantConstants.OrganizationId
            });

            context.TaskTypes.Add(new TaskType("Not billable", null)
            {
                OrganizationId = TenantConstants.OrganizationId
            });

            await context.SaveChangesAsync();
        }

        if (!context.AbsenceTypes.Any())
        {
            context.AbsenceTypes.Add(new AbsenceType()
            {
                OrganizationId = TenantConstants.OrganizationId,
                Name = "Vacation",
                FullDays = true
            });

            context.AbsenceTypes.Add(new AbsenceType
            {
                OrganizationId = TenantConstants.OrganizationId,
                Name = "Sick leave"
            });

            context.AbsenceTypes.Add(new AbsenceType
            {
                OrganizationId = TenantConstants.OrganizationId,
                Name = "Compensatory leave"
            });

            await context.SaveChangesAsync();
        }

        return;

        /*

        var project = new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Internal",

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        context.Projects.Add(project);

        var task = new Task
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Documentation",
            MinHoursPerDay = null,
            MaxHoursPerDay = null,

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        project.Tasks.Add(task);

        var task3 = new Task
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Miscellaneous",
            MinHoursPerDay = null,
            MaxHoursPerDay = null,

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        project.Tasks.Add(task3);

        var project2 = new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ACME",

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        context.Projects.Add(project2);

        var task2 = new Task
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Project time",

            HourlyRate = 640m,

            MinHoursPerDay = null,
            MaxHoursPerDay = null,

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        project2.Tasks.Add(task2);

        await context.SaveChangesAsync();

        var user1 = new User()
        {
            Id = Guid.NewGuid().ToString(),
            SSN = "sdfsdfsd",
            FirstName = "Alice",
            LastName = "McDonald",

            Email = "alice.mcdonald@onestop.io",

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        var user2 = new User()
        {
            Id = Guid.NewGuid().ToString(),
            SSN = "sfsdfsdf",
            FirstName = "Robert",
            LastName = "Johnson",
            DisplayName = "Bob Johnson",

            Email = "bob.johnson@onestop.io",

            Created = DateTime.Now,
            CreatedById = "N/A"
        };

        context.Users.AddRange(new User[] {
                user1, user2
            });

        await context.SaveChangesAsync();

        project.Memberships.Add(new ProjectMembership()
        {
            Id = Guid.NewGuid().ToString(),
            User = user1,

            Created = DateTime.Now,
            CreatedById = "N/A"
        });

        project.Memberships.Add(new ProjectMembership()
        {
            Id = Guid.NewGuid().ToString(),
            User = user2,

            Created = DateTime.Now,
            CreatedById = "N/A"
        });

        project2.Memberships.Add(new ProjectMembership()
        {
            Id = Guid.NewGuid().ToString(),
            User = user2,

            Created = DateTime.Now,
            CreatedById = "N/A"
        });

        await context.SaveChangesAsync();

        */
    }
}