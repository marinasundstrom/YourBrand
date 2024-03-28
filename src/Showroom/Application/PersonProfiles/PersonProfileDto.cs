using System;

using YourBrand.Showroom.Application.CompetenceAreas;
using YourBrand.Showroom.Application.Industries;
using YourBrand.Showroom.Application.Organizations;

namespace YourBrand.Showroom.Application.PersonProfiles;

public record PersonProfileDto(
    string Id,
    string FirstName,
    string LastName,
    string? DisplayName,
    DateTime? BirthDate,
    string? Location,
    IndustryDto Industry,
    OrganizationDto Organization,
    CompetenceAreaDto CompetenceArea,
    string? ProfileImage,
    string Headline,
    string ShortPresentation,
    string Presentation,
    string? ProfileVideo,
    DateTime? AvailableFromDate,
    string? Email,
    string? PhoneNumber
);
