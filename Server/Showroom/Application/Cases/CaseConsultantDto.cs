
using Skynet.Showroom.Application.ConsultantProfiles;

namespace Skynet.Showroom.Application.Cases;

public record CaseConsultantDto
(
    string Id,
    ConsultantProfileDto Consultant,
    string? Presentation
);
