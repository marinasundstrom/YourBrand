using YourBrand.Showroom.Application.Common.Models;

namespace YourBrand.Showroom.Application.Organizations;

public record OrganizationDto
(
    string Id,
    string Name,
    AddressDto Address
);