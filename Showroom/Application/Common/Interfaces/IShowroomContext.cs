using YourBrand.Showroom.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourBrand.Showroom.Application.Common.Interfaces;

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

    DbSet<ConsultantProfileEducation> ConsultantProfileEducation { get; }

    DbSet<ConsultantProfileLanguage> ConsultantProfileLanguages { get; }

    DbSet<Language> Languages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}