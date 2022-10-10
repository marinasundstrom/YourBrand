using System;

using YourBrand.Customers.Application.Addresses;

namespace YourBrand.Customers.Application.Organizations;

public record OrganizationDto(int Id, string Name, string OrgNo, bool HasCeased, string? Phone, string? PhoneMobile, string? Email, IEnumerable<AddressDto> Addresses);
