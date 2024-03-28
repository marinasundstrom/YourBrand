
using YourBrand.Showroom.Application.PersonProfiles;

namespace YourBrand.Showroom.Application.Cases;

public record CaseProfileDto
(
    string Id,
    PersonProfileDto Profile,
    string? Presentation
);
