using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.ApiKeys;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Infrastructure.Persistence.Interceptors;
using YourBrand.Showroom.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Infrastructure.Persistence;

public class ShowroomContext(
    DbContextOptions<ShowroomContext> options,
    ITenantContext tenantContext) : DbContext(options), IShowroomContext
{
    public TenantId? TenantId => tenantContext.TenantId;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShowroomContext).Assembly);

        modelBuilder.ConfigureDomainModel(configurator =>
        {
            configurator.AddTenancyFilter(() => TenantId);
            configurator.AddSoftDeleteFilter();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }


#nullable disable

    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<PersonProfile> PersonProfiles { get; set; } = null!;

    public DbSet<CompetenceArea> CompetenceAreas { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<Employment> Employments { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Case> Cases { get; set; } = null!;

    public DbSet<CaseCandidateProfile> CaseCandidateProfiles { get; set; } = null!;

    public DbSet<SkillArea> SkillAreas { get; set; } = null!;

    public DbSet<Skill> Skills { get; set; } = null!;

    public DbSet<PersonProfileSkill> PersonProfileSkills { get; set; } = null!;

    public DbSet<PersonProfileExperience> PersonProfileExperiences { get; set; } = null!;

    public DbSet<PersonProfileExperienceSkill> PersonProfileExperienceSkills { get; set; } = null!;

    public DbSet<PersonProfileEducation> PersonProfileEducation { get; set; } = null!;

    public DbSet<PersonProfileLanguage> PersonProfileLanguages { get; set; } = null!;

    public DbSet<PersonProfileIndustryExperiences> PersonProfileIndustryExperiences { get; set; } = null!;

    public DbSet<Language> Languages { get; set; } = null!;

    public DbSet<Industry> Industries { get; set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<IHasDomainEvents>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        if (!entities.Any())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}