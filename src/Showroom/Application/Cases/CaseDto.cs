namespace YourBrand.Showroom.Application.Cases;

public record CaseDto
(
    string Id,
    string OrganizationId,
    string Status,
    string? Description,
    IEnumerable<CaseProfileDto> CaseProfiles,
    CasePricingDto Pricing
);

public record CasePricingDto(decimal? HourlyPrice, double? Hours, decimal? Total);