using YourBrand.Showroom.Application.Industries;

namespace YourBrand.Showroom.Application.Skills;

public record SkillAreaDto
(
    string Id,
    string Name,
    IndustryDto Industry
);
