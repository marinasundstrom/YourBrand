using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Application.Cases;

public record CaseDto
(
    string Id,
    string Status,
    string? Description,
    IEnumerable<CaseConsultantDto> Consultants
);
