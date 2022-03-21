using System;

using YourCompany.Showroom.Application.CompetenceAreas;
using YourCompany.Showroom.Application.Organizations;

namespace YourCompany.Showroom.Application.ConsultantProfiles;

public record ConsultantProfileDto(
    string Id,
    string FirstName,
    string LastName,
    string? DisplayName,
    DateTime? BirthDate,
    string? Location,
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
