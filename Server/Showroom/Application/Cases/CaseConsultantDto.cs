
using YourCompany.Showroom.Application.ConsultantProfiles;

namespace YourCompany.Showroom.Application.Cases;

public record CaseConsultantDto
(
    string Id,
    ConsultantProfileDto Consultant,
    string? Presentation
);
