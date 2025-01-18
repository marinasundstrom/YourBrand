using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.ValueObjects;
using YourBrand.Showroom.Events.Enums;
using YourBrand.Showroom.TestData;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<ShowroomContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await LoadIndustries(context);

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
            context.Users.Add(new User("api")
            {
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
                Name = "Electrical Engineering"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Name = "Mechanical Engineering"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Name = "Project Management"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Name = "Quality Assurance (QA)"
            });

            context.CompetenceAreas.Add(new CompetenceArea
            {
                Name = "Software Engineering"
            });

            await context.SaveChangesAsync();
        }

        /*

        if (!context.SkillAreas.Any())
        {
            var area1 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Software Architecture",
                Slug = "software-architecture"
            };

            area1.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Microservices",
                Slug = "microservices"
            });

            area1.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Event-driven Architecture",
                Slug = "event-driven-architecture"
            });

            context.SkillAreas.Add(area1);

            var area2 = new SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Development Platforms",
                Slug = "development-platforms"
            };

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = ".NET",
                Slug = "net"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Java",
                Slug = "java"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Node",
                Slug = "node"
            });

            area2.Skills.Add(new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Web",
                Slug = "web"
            });

            context.SkillAreas.Add(area2);

            await context.SaveChangesAsync();
        }
        */

        PersonProfile personProfile = new PersonProfile()
        {
            OrganizationId = TenantConstants.OrganizationId,
            FirstName = "Marina",
            LastName = "Sundström",
            DisplayName = null,
            BirthDate = new DateTime(1990, 1, 5),
            Industry = await context.Industries.FirstAsync(x => x.Name.Contains("Software Development")),
            Organization = await context.Organizations.FirstAsync(),
            CompetenceArea = await context.CompetenceAreas.FirstAsync(x => x.Name.Contains("Software")),
            Headline = "Experienced Software Developer",
            ShortPresentation = "I'm as Software developer who is based in Malmö, Sweden.",
            Presentation = @"
I'm as Software developer who is based in Malmö, Sweden.

I have been programming since 2007. My interest back then was to figure out how it all works. Since then I have been learning a lot about software engineering, both professionally and in my free time.

My career began back in 2014, when I was working as a software developer for a local company that provided Internet and IT services. After that I went into consulting, where I got to experience software development at various companies and in many fields. I have maninly been working with technologies such as .NET and Web.",
        };

        context.PersonProfiles.Add(personProfile);

        await LoadTestData(context, personProfile);

        await context.SaveChangesAsync();

        /*
        personProfile.Languages.Add(new LanguageSkill {
            Language = context.Languages.FirstOrDefault(l => l.ISO639 == "sv"),
            SkillLevel = SkillLevel.Native
        });
        */
    }

    private static async Task LoadTestData(ShowroomContext context, PersonProfile personProfile)
    {
        var skillGroups = Skills2.FromJson(await File.ReadAllTextAsync("../TestData/skills.json"));
        foreach (var skillGroup in skillGroups)
        {
            var skillArea = new Domain.Entities.SkillArea()
            {
                Name = skillGroup.Key,
                Slug = GetSkillName(skillGroup.Key),
                Industry = await context.Industries.FirstAsync(x => x.Name == "Software Development"),
            };

            foreach (var skillPair in skillGroup.Value)
            {
                var skillName = skillPair.Key;
                var skillInfo = skillPair.Value;

                var skill = new Domain.Entities.Skill()
                {
                    Name = skillName,
                    Slug = GetSkillName(skillName),
                };

                skillArea.Skills.Add(skill);

                personProfile.PersonProfileSkills.Add(new PersonProfileSkill
                {
                    Skill = skill,
                    //Years = skillInfo.Years,
                    Level = (Domain.Enums.SkillLevel?)skillInfo.Level,
                    Comment = skillInfo.Comment,
                    Link = skillInfo.Link is null ? null : new Domain.ValueObjects.Link(skillInfo.Link.Title, skillInfo.Link.Href)
                });
            }

            context.SkillAreas.Add(skillArea);

            await context.SaveChangesAsync();
        }

        var resume = Resume.FromJson(await File.ReadAllTextAsync("../TestData/resume.json"));
        foreach (var experience in resume.Experience)
        {
            var company = await context.Companies.FirstOrDefaultAsync(x => x.Name == experience.Company);

            if (company is null)
            {
                company = new Company()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = experience.Company,
                    Industry = await context.Industries.FirstAsync(x => x.Name == experience.Industry),
                    Logo = experience.CompanyLogo,
                    Link = experience.Link,
                };

                context.Companies.Add(company);

                await context.SaveChangesAsync();
            }
        }

        foreach (var experience in resume.Experience)
        {
            var company = await context.Companies.FirstAsync(x => x.Name == experience.Company);

            var employment = await context.Employments.FirstOrDefaultAsync(x => x.Employer.Name == experience.Employer);

            if (employment is null)
            {
                employment = new Employment()
                {
                    Employer = await context.Companies.FirstAsync(x => x.Name == experience.Employer),
                    StartDate = resume.Experience.OrderBy(x => x.StartDate).First(x => x.Company == experience.Company).StartDate,
                    EndDate = resume.Experience.OrderBy(x => x.StartDate).Last(x => x.Company == experience.Company).EndDate,
                    EmploymentType = EmploymentType.Parse<EmploymentType>(experience.EmploymentType),
                    Description = null, // experience.Description,
                    Location = experience.Location
                };

                employment.Roles.Add(new EmploymentRole()
                {
                    PersonProfile = personProfile,
                    Title = experience.Title,
                    Location = experience.Location,
                    StartDate = resume.Experience.OrderBy(x => x.StartDate).First(x => x.Company == experience.Company).StartDate,
                    EndDate = resume.Experience.OrderBy(x => x.StartDate).Last(x => x.Company == experience.Company).EndDate,
                    Description = experience.Description
                });

                personProfile.Employments.Add(employment);

                foreach (var skill in experience.Skills)
                {
                    var name = GetSkillName(skill);

                    var sk = await context.PersonProfileSkills.FirstOrDefaultAsync(x => x.Skill.Slug == name);

                    if (sk is null)
                    {
                        var sk2 = await context.Skills.FirstOrDefaultAsync(x => x.Slug == name);

                        if (sk2 is null) continue;

                        sk = new PersonProfileSkill()
                        {
                            PersonProfile = personProfile,
                            Skill = sk2!
                        };

                        personProfile.PersonProfileSkills.Add(sk);
                    }

                    employment.Skills.Add(new PersonProfileExperienceSkill()
                    {
                        Employment = employment,
                        PersonProfileSkill = sk
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        foreach (var experience in resume.Experience.Where(x => x.EmploymentType == "Contract"))
        {
            var company = await context.Companies
                .Include(x => x.Industry)
                .FirstAsync(x => x.Name == experience.Company);

            var employment = await context.Employments.FirstOrDefaultAsync(x => x.Employer.Name == experience.Employer);

            var assignment = new Assignment()
            {
                PersonProfile = personProfile,
                Company = company,
                Location = experience.Location,
                Employment = employment,
                AssignmentType = AssignmentType.Consulting,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = null
            };

            assignment.Roles.Add(new EmploymentRole()
            {
                PersonProfile = personProfile,
                Employment = employment,
                Assignment = assignment,
                Title = experience.Title,
                Location = experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                Description = experience.Description
            });


            //experience2.AddDomainEvent(new ExperienceAdded(experience2.Id, personProfile.Id, company.Industry.Id));

            foreach (var skill in experience.Skills)
            {
                var name = GetSkillName(skill);

                var sk = await context.PersonProfileSkills.FirstOrDefaultAsync(x => x.Skill.Slug == name);

                if (sk is null)
                {
                    var sk2 = await context.Skills.FirstOrDefaultAsync(x => x.Slug == name);

                    if (sk2 is null) continue;

                    sk = new PersonProfileSkill()
                    {
                        PersonProfile = personProfile,
                        Skill = sk2!
                    };

                    personProfile.PersonProfileSkills.Add(sk);
                }

                assignment.Skills.Add(new PersonProfileExperienceSkill()
                {
                    //Employment = employment,
                    Assignment = assignment,
                    PersonProfileSkill = sk
                });
            }

            personProfile.Assignments.Add(assignment);
        }

        await context.SaveChangesAsync();
    }

    private static string GetSkillName(string skillName)
    {
        return skillName
                            .ToLower()
                            .Replace(" ", "-")
                            .Replace("(", string.Empty)
                            .Replace(")", string.Empty)
                            .Replace(".", string.Empty)
                            .Replace("#", string.Empty);
    }

    private static async Task LoadIndustries(ShowroomContext context)
    {
        var industries = JsonDocument.Parse(await File.ReadAllTextAsync("../Infrastructure/industries.json"));
        foreach (var i in industries.RootElement.EnumerateArray())
        {
            var sk = new Industry()
            {
                Name = i.GetProperty("Label").GetString()!
            };

            context.Industries.Add(sk);
        }

        await context.SaveChangesAsync();
    }
}