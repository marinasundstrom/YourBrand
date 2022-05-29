using System;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using YourBrand.ApiKeys;

namespace YourBrand.Showroom.Infrastructure.Persistence;

class ShowroomContext : DbContext, IShowroomContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;
    private readonly IApiApplicationContext _apiApplicationContext;
    private Transaction? _transaction;

    public ShowroomContext(
        DbContextOptions<ShowroomContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTime dateTime,
        IApiApplicationContext apiApplicationContext) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;
        _apiApplicationContext = apiApplicationContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); //.LogTo(Console.WriteLine);
#endif

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.ConsultantProfilCaseConfigurationConfiguration).Assembly);
    }

#nullable disable

    public DbSet<ConsultantProfile> ConsultantProfiles { get; set; } = null!;

    public DbSet<CompetenceArea> CompetenceAreas { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Case> Cases { get; set; } = null!;

    public DbSet<CaseConsultant> CaseConsultants { get; set; } = null!;

    public DbSet<SkillArea> SkillAreas { get; set; } = null!;

    public DbSet<Skill> Skills { get; set; } = null!;

    public DbSet<ConsultantProfileSkill> ConsultantProfileSkills { get;  set; } = null!;

    public DbSet<ConsultantProfileExperience> ConsultantProfileExperiences { get;  set; } = null!;

    public DbSet<ConsultantProfileEducation> ConsultantProfileEducation { get;  set; } = null!;

    public DbSet<ConsultantProfileLanguage> ConsultantProfileLanguages { get;  set; } = null!;

    public DbSet<Language> Languages { get;  set; } = null!;

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            entry.Entity.ApplicationId = _apiApplicationContext.AppId;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedById = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedById = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedById = _currentUserService.UserId;
                        softDelete.Deleted = _dateTime.Now;

                        entry.State = EntityState.Modified;
                    }
                    break;
            }
        }

        if (_transaction is not null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        DomainEvent[] events = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    private DomainEvent[] GetDomainEvents()
    {
        return ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(domainEvent => !domainEvent.IsPublished)
            .ToArray();
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var transaction = await Database.BeginTransactionAsync();

        _transaction = new Transaction(
            this,
            transaction);

        return _transaction;
    }

    class Transaction : ITransaction
    {
        private readonly ShowroomContext _context;
        private readonly IDbContextTransaction _transaction;

        public Transaction(ShowroomContext context, IDbContextTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public async Task CommitAsync()
        {
            DomainEvent[] events = _context.GetDomainEvents();

            await _transaction.CommitAsync();

            await _context.DispatchEvents(events);
            _context._transaction = null;
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _context._transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            _context._transaction = null;
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            _context._transaction = null;
        }
    }
}