using System;

using YourBrand.Showroom.Application.CompetenceAreas;
using YourBrand.Showroom.Application.Organizations;

namespace YourBrand.Showroom.Application.Companies;

public record CompanyDto(
    string Id,
    string Name,
    string? Logo, 
    string? Link
);
