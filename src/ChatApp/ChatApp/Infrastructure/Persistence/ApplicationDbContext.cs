using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence.ValueConverters;
using YourBrand.Domain;
using YourBrand.Domain.Outbox;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly ITenantContext tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        this.tenantContext = tenantContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(IEntity)))
            {
                Console.WriteLine($"Skipping type {clrType} because it is not implementing IEntity.");
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            entityTypeBuilder.AddTenantIndex();

            entityTypeBuilder.AddOrganizationIndex();

            try
            {
                entityTypeBuilder.RegisterQueryFilters(builder =>
                {
                    builder.AddTenancyFilter(tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
                            when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ChannelId>().HaveConversion<ChannelIdConverter>();
        configurationBuilder.Properties<ChannelParticipantId>().HaveConversion<ChannelParticipantIdConverter>();
        configurationBuilder.Properties<MessageId>().HaveConversion<MessageIdConverter>();

        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Channel> Channels { get; set; }

    public DbSet<ChannelParticipant> ChannelParticipants { get; set; }

    public DbSet<Message> Messages { get; set; }

    //public DbSet<MessageReaction> MessageReactions { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }

#nullable restore
}