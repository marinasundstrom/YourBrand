using System;

using YourBrand.Marketing.Application.Campaigns;

namespace YourBrand.Marketing.Application.Contacts;

public record ContactDto(string Id, CampaignDto? Campaign, string FirstName, string LastName, string SSN, string? Phone, string? PhoneMobile, string? Email, ContactAddressDto? Address);
