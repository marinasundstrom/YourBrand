using YourBrand.Marketing.Domain.Enums;

namespace YourBrand.Marketing.Application.Contacts;

public record ContactAddressDto(
    // Street
    string Thoroughfare,

    // Street number
    string? Premises,

    // Suite
    string? SubPremises,

    string PostalCode,

    // Town or City
    string Locality,

    // County
    string SubAdministrativeArea,

    // State
    string AdministrativeArea,

    string Country
);