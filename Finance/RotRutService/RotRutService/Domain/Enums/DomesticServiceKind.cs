using System.ComponentModel.DataAnnotations;

namespace YourBrand.RotRutService.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DomesticServiceKind
{
    [Display(Name = "ROT")]
    HomeRepairAndMaintenanceServiceType = 1,

    [Display(Name = "RUT")]
    HouseholdService = 2
}
