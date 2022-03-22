using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Application.Skills;

public record SkillDto
(
    string Id,
    string Name,
    SkillAreaDto Area
);
