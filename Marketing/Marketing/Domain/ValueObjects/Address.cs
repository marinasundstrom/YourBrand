namespace YourBrand.Marketing.Domain.ValueObjects;

public record Address(
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