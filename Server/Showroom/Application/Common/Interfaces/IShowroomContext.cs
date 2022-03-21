using YourCompany.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourCompany.Showroom.Application.Common.Interfaces;

public interface IShowroomContext
{
    DbSet<ConsultantProfile> ConsultantProfiles { get; }

    DbSet<CompetenceArea> CompetenceAreas { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<User> Users { get; }

    DbSet<Case> Cases { get; }

    DbSet<CaseConsultant> CaseConsultants { get; }

    DbSet<SkillArea> SkillAreas { get; }

    DbSet<Skill> Skills { get; }

    DbSet<ConsultantProfileSkill> ConsultantProfileSkills { get; }

    DbSet<ConsultantProfileExperience> ConsultantProfileExperiences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync();
}

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}