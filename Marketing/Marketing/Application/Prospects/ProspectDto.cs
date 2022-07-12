using System;

using YourBrand.Marketing.Application.Addresses;

namespace YourBrand.Marketing.Application.Prospects;

public record ProspectDto(string Id, string FirstName, string LastName, string SSN, string? PhoneHome, string? PhoneMobile, string? Email);
