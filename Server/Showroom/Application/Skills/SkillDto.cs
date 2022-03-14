using System.ComponentModel.DataAnnotations;

namespace Skynet.Showroom.Application.Skills;

public record SkillDto
(
    string Id,
    string Name,
    SkillAreaDto Area
);
