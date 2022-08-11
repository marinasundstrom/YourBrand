using YourBrand.Showroom.Application.Skills;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

public record ConsultantProfileSkillExperiencesDto(string Id, SkillDto Skill, SkillLevel? Level, string? Comment, LinkDto? Link, IEnumerable<SkillExperienceDto> Experiences);
