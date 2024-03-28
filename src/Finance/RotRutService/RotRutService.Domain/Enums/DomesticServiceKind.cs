using System.ComponentModel.DataAnnotations;

namespace YourBrand.RotRutService.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DomesticServiceKind
{
    [Display(Name = "ROT")]
    HomeRepairAndMaintenanceServiceType = 0,

    [Display(Name = "RUT")]
    HouseholdService = 1
}
