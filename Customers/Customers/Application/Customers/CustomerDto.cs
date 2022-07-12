using System;

using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Domain.Enums;

namespace YourBrand.Customers.Application.Customers;

public record CustomerDto(int Id, CustomerType CustomerType, string Name, string? SSN, string? OrgNo, string? VATNo);
