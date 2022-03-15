using System;

using Skynet.Showroom.Application.CompetenceAreas;
using Skynet.Showroom.Application.Organizations;

namespace Skynet.Showroom.Application.ConsultantProfiles;

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
