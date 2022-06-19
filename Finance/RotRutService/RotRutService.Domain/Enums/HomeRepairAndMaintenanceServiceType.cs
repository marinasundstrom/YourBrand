using System.ComponentModel.DataAnnotations;

namespace YourBrand.RotRutService.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum HomeRepairAndMaintenanceServiceType
{
    [Display(Name = "Bygg")]
    Carpentry,

    [Display(Name = "El")]
    ElectricityWork,

    [Display(Name = "Glas och metalarbete")]
    GlassMetalWork,

    [Display(Name = "Tapetsering")]
    Wallpapering,

    [Display(Name = "Dränering")]
    DrainageWork,

    [Display(Name = "Murning")]
    Masonry,

    [Display(Name = "Vvs")]
    WaterAndHeating
}
