using YourBrand.Marketing.Domain.Enums;

namespace YourBrand.Marketing.Application.Addresses;

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