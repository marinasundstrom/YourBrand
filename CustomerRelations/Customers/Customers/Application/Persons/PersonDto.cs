using System;

using YourBrand.Customers.Application.Addresses;

namespace YourBrand.Customers.Application.Persons;

public record PersonDto(int Id, string FirstName, string LastName, string SSN, bool IsDeceased, string? Phone, string? PhoneMobile, string? Email, IEnumerable<AddressDto> Addresses);
