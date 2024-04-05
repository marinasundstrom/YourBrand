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

public class ShowroomContext : DbContext, IShowroomContext
{
    private readonly IApiApplicationContext _apiApplicationContext;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly TenantId _tenantId;

    public ShowroomContext(
        DbContextOptions<ShowroomContext> options,
        IApiApplicationContext apiApplicationContext,
        ITenantContext tenantContext,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _apiApplicationContext = apiApplicationContext;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;

        _tenantId = tenantContext.TenantId.GetValueOrDefault();
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.PersonProfilCaseConfigurationConfiguration).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (clrType.BaseType != typeof(object))
            {
                continue;
            }

            try
            {
                var entityTypeBuilder = modelBuilder.Entity(clrType);

                var parameter = Expression.Parameter(clrType, "entity");

                List<Expression> queryFilters = new();

                if (TenancyQueryFilter.CanApplyTo(clrType))
                {
                    var tenantFilter = TenancyQueryFilter.GetFilter(() => _tenantId!);

                    queryFilters.Add(
                        Expression.Invoke(tenantFilter, Expression.Convert(parameter, typeof(IHasTenant))));
                }

                if (SoftDeleteQueryFilter.CanApplyTo(clrType))
                {
                    var softDeleteFilter = SoftDeleteQueryFilter.GetFilter();

                    queryFilters.Add(
                        Expression.Invoke(softDeleteFilter, Expression.Convert(parameter, typeof(ISoftDelete))));
                }

                Expression? queryFilter = null;

                foreach (var qf in queryFilters)
                {
                    if (queryFilter is null)
                    {
                        queryFilter = qf;
                    }
                    else
                    {
                        queryFilter = Expression.AndAlso(
                            queryFilter,
                            qf)
                            .Expand();
                    }
                }

                if (queryFilters.Count == 0)
                {
                    continue;
                }

                var queryFilterLambda = Expression.Lambda(queryFilter.Expand(), parameter);

                entityTypeBuilder.HasQueryFilter(queryFilterLambda);
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be configured as non-owned because it has already been configured as a owned"))
            {
                Console.WriteLine("Skipping previously configured owned type");
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be added to the model because its CLR type has been configured as a shared type"))
            {
                Console.WriteLine("Skipping previously configured shared type");
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
    }


#nullable disable

    public DbSet<Company> Companies { get; set; } = null!;

    public DbSet<PersonProfile> PersonProfiles { get; set; } = null!;

    public DbSet<CompetenceArea> CompetenceAreas { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<Employment> Employments { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Case> Cases { get; set; } = null!;

    public DbSet<CaseProfile> CaseProfiles { get; set; } = null!;

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
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
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