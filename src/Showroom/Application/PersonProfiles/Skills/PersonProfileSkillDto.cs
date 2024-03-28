using YourBrand.Showroom.Application.Skills;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

public record PersonProfileSkillDto(string Id, SkillDto Skill, SkillLevel? Level, string? Comment, LinkDto? Link);
