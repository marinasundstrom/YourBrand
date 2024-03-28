using YourBrand.Customers.Domain.Enums;

namespace YourBrand.Customers.Application.Addresses;

public record AddressDto(
    string Id,

    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    AddressType Type,

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

public record Address2Dto(
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