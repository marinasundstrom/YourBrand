using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.ValueObjects;
using YourBrand.Showroom.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;
using YourBrand.Showroom.TestData;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Showroom.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ShowroomContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Languages.Any())
        {
            var language = new Language
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Swedish",
                NativeName = "svenska",
                ISO639 = "sv"
            };

            context.Languages.Add(language);

        language = new Language
            {
                Id = Guid.NewGuid().ToString(),
                Name = "English",
                NativeName = "English",
                ISO639 = "en"
            };

            context.Languages.Add(language);
        }

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

        ConsultantProfile consultantProfile = new ConsultantProfile() {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Marina",
            LastName = "Sundström",
            DisplayName = null,
            BirthDate = new DateTime(1990, 1, 5),
            Organization = await context.Organizations.FirstAsync(),
            CompetenceArea = await context.CompetenceAreas.FirstAsync(),
            Headline = "Senior Software Developer",
            ShortPresentation = "I'm as Software developer who is based in Malmö, Sweden.",
            Presentation = @"
I'm as Software developer who is based in Malmö, Sweden.

I have been programming since 2007. My interest back then was to figure out how it all works. Since then I have been learning a lot about software engineering, both professionally and in my free time.

My career began back in 2014, when I was working as a software developer for a local company that provided Internet and IT services. After that I went into consulting, where I got to experience software development at various companies and in many fields. I have maninly been working with technologies such as .NET and Web.",
        };

        context.ConsultantProfiles.Add(consultantProfile);

        var resume = Resume.FromJson(await File.ReadAllTextAsync("../TestData/resume.json"));
        foreach(var experience in resume.Experience)
        {
            consultantProfile.Experience.Add(new Domain.Entities.ConsultantProfileExperience() {
                Id = Guid.NewGuid().ToString(),
                ConsultantProfile = consultantProfile,
                Current = experience.Current,
                Highlight = experience.Highlight,
                CompanyName = experience.Company,
                CompanyLogo = experience.CompanyLogo,
                Link = experience.Link,
                Location = experience.Location,
                Title = experience.Title,
                EmploymentType = experience.EmploymentType,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description
            });
        }

        await context.SaveChangesAsync();

        /*
        consultantProfile.Languages.Add(new LanguageSkill {
            Language = context.Languages.FirstOrDefault(l => l.ISO639 == "sv"),
            SkillLevel = SkillLevel.Native
        });
        */
    }
}