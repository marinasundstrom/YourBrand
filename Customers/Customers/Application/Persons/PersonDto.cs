using System;

using YourBrand.Customers.Application.Addresses;

namespace YourBrand.Customers.Application.Persons;

public record PersonDto(string Id, string FirstName, string LastName, string SSN, string? PhoneHome, string PhoneMobile, string Email, IEnumerable<AddressDto> Addresses);
