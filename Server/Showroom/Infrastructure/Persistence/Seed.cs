using Skynet.Showroom.Domain.Entities;
using Skynet.Showroom.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace Skynet.Showroom.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ShowroomContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Users.Any())
        {
            context.Users.Add(new User {
                Id = "api",
                FirstName = "API",
                LastName = "User",
                SSN = "213",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        if (!context.Organizations.Any())
        {
            context.Organizations.Add(new Organization
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Organization",
                Address = new Address
                {
                    Address1 = "",
                    Address2 = "",
                    PostalCode = "",
                    Locality = "",
                    SubAdminArea = "",
                    AdminArea = "",
                    Country = ""
                },
            });

            await context.SaveChangesAsync();
        }

        if (!context.CompetenceAreas.Any())
        {
            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 1"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 2"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Area 3"
            });

            await context.SaveChangesAsync();
        }

        if (!context.SkillAreas.Any())
        {
            var area1 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Software Architecture"
            };

            area1.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = "Micro services"
            });

            area1.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = "Event-driven Architecture"
            });

            context.SkillAreas.Add(area1);

            var area2 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Development Platforms"
            };

            area2.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = ".NET"
            });

            area2.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = "Java"
            });

            area2.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = "Node"
            });

            area2.Skills.Add(new Skill {
                Id = Guid.NewGuid().ToString(),
                Name = "Web"
            });

            context.SkillAreas.Add(area2);

            await context.SaveChangesAsync();
        }
    }
}