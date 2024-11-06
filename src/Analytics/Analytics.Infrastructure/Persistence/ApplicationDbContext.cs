using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Domain.Persistence;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Analytics.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext) : DbContext(options), IApplicationDbContext
{
    public TenantId? TenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ApplyDomainEntityConfigurations()
            .ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

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

    public DbSet<Client> Clients { get; set; } = null!;

    public DbSet<Session> Sessions { get; set; } = null!;

    public DbSet<Event> Events { get; set; } = null!;

#nullable restore
}