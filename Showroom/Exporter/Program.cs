// See https://aka.ms/new-console-template for more information
using System.Linq;
using System.Text.Json;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using YourBrand.ApiKeys;
using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Infrastructure;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = builder.Build();

ServiceCollection services = new ();
services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(Program)));
services.AddLogging();
services.AddInfrastructure(configuration);
services.AddScoped<IApiApplicationContext>(sp => Substitute.For<IApiApplicationContext>());
services.AddScoped<ICurrentUserService>(sp => Substitute.For<ICurrentUserService>());

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<IShowroomContext>();

var experiences = await context.PersonProfileExperiences
    .Include(x => x.Employment)
    .Include(x => x.Company)
    .ThenInclude(x => x.Industry)
    .Include(x => x.Skills)
    .ThenInclude(x => x.PersonProfileSkill)
    .ThenInclude(x => x.Skill)
    .ToArrayAsync();

var experiences2 = experiences
    .OrderByDescending(x => x.StartDate)
    .ThenBy(x => x.Current)
    .Select(experience => 
    new
    {
        Title = experience.Title,
        Employer = experience.Employment.Employer.Name,
        EmploymentType = experience.EmploymentType.ToString(),
        Highlight = experience.Highlight,
        Company = experience.Company.Name,
        CompanyLogo = experience.Company.Logo,
        Industry = experience.Company.Industry.Name,
        Link = experience.Company.Link,
        Location = experience.Location,
        Current = experience.Current,
        StartDate = experience.StartDate,
        EndDate = experience.EndDate,
        Description = experience.Description,
        Skills = experience.Skills.Select(x => x.PersonProfileSkill.Skill.Name)
    });

Console.WriteLine(JsonSerializer.Serialize(experiences2, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));