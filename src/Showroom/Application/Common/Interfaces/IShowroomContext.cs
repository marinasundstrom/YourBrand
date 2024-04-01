using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface IShowroomContext
{
    DbSet<Company> Companies { get; }

    DbSet<PersonProfile> PersonProfiles { get; }

    DbSet<CompetenceArea> CompetenceAreas { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<Employment> Employments { get; }

    DbSet<User> Users { get; }

    DbSet<Case> Cases { get; }

    DbSet<CaseProfile> CaseProfiles { get; }

    DbSet<SkillArea> SkillAreas { get; }

    DbSet<Skill> Skills { get; }

    DbSet<PersonProfileSkill> PersonProfileSkills { get; }

    DbSet<PersonProfileExperience> PersonProfileExperiences { get; }

    DbSet<PersonProfileExperienceSkill> PersonProfileExperienceSkills { get; }

    DbSet<PersonProfileEducation> PersonProfileEducation { get; }

    DbSet<PersonProfileLanguage> PersonProfileLanguages { get; }

    DbSet<PersonProfileIndustryExperiences> PersonProfileIndustryExperiences { get; }

    DbSet<Language> Languages { get; }

    DbSet<Industry> Industries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}