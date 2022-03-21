using System.ComponentModel.DataAnnotations;

namespace YourCompany.Showroom.Application.Skills;

public record SkillDto
(
    string Id,
    string Name,
    SkillAreaDto Area
);
