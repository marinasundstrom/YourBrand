using System;
namespace YourCompany.Showroom.Application.Common.Models;

public record AddressDto
(
    string Address1,
    string Address2,
    string PostalCode,
    string Locality,
    string SubAdminArea,
    string AdminArea,
    string Country
);

