using System.ComponentModel.DataAnnotations;

namespace YourCompany.Showroom.Application.Cases;

public record CaseDto
(
    string Id,
    string Status,
    string? Description,
    IEnumerable<CaseConsultantDto> Consultants
);
