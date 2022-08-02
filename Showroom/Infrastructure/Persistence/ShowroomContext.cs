using System;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using YourBrand.ApiKeys;
using YourBrand.Showroom.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Showroom.Infrastructure.Persistence;

class ShowroomContext : DbContext, IShowroomContext
{
    private readonly IDomainEventService _domainEventService;
    private readonly IApiApplicationContext _apiApplicationContext;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ShowroomContext(
        DbContextOptions<ShowroomContext> options,
        IDomainEventService domainEventService,
        IApiApplicationContext apiApplicationContext,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _domainEventService = domainEventService;
        _apiApplicationContext = apiApplicationContext;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.ConsultantProfilCaseConfigurationConfiguration).Assembly);
    }

#nullable disable

    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<ConsultantProfile> ConsultantProfiles { get; set; } = null!;

    public DbSet<CompetenceArea> CompetenceAreas { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<Employment> Employments { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Case> Cases { get; set; } = null!;

    public DbSet<CaseConsultant> CaseConsultants { get; set; } = null!;

    public DbSet<SkillArea> SkillAreas { get; set; } = null!;

    public DbSet<Skill> Skills { get; set; } = null!;

    public DbSet<ConsultantProfileSkill> ConsultantProfileSkills { get;  set; } = null!;

    public DbSet<ConsultantProfileExperience> ConsultantProfileExperiences { get;  set; } = null!;

    public DbSet<ConsultantProfileExperienceSkill> ConsultantProfileExperienceSkills { get;  set; } = null!;

    public DbSet<ConsultantProfileEducation> ConsultantProfileEducation { get;  set; } = null!;

    public DbSet<ConsultantProfileLanguage> ConsultantProfileLanguages { get;  set; } = null!;

    public DbSet<Language> Languages { get;  set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _domainEventService.Publish(domainEvent);
    }
}