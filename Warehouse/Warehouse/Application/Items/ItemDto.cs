using System;

namespace YourBrand.Warehouse.Application.Items;

public record ItemDto(string Id, string FirstName, string LastName, string SSN, string? PhoneHome, string? PhoneMobile, string? Email);
