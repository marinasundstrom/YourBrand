using YourBrand.Showroom.Application.Skills;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

public record ConsultantProfileSkillDto(string Id, SkillDto Skill, SkillLevel? Level, string? Comment, LinkDto? Link);
