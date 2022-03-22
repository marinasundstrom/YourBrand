
using YourBrand.Showroom.Application.ConsultantProfiles;

namespace YourBrand.Showroom.Application.Cases;

public record CaseConsultantDto
(
    string Id,
    ConsultantProfileDto Consultant,
    string? Presentation
);
