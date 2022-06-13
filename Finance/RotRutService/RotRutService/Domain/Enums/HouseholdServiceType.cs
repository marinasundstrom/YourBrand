using System.ComponentModel.DataAnnotations;

namespace YourBrand.RotRutService.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum HouseholdServiceType
{
    [Display(Name = "Barnpassning")]
    Childcare,

    [Display(Name = "Flyttjänst")]
    MovingService,

    [Display(Name = "It-tjänster")]
    ItServices,

    [Display(Name = "Kläd och textilvård")]
    ClothingAndTextileCare,

    [Display(Name = "Personlig omsorg")]
    PersonalCare,

    [Display(Name = "Reparation av vitvaror")]
    RepairOfHoushouldAppliances,

    [Display(Name = "Snöskottning")]
    SnowShoveling,

    [Display(Name = "Städning")]
    Cleaning,

    [Display(Name = "Trädgårdsarbete")]
    Gardening
}