using System.ComponentModel.DataAnnotations;

namespace YourBrand.RotRutService.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PropertyType
{
    [Display(Name = "Villa")]
    HousingUnit = 0,

    [Display(Name = "Bostadsrätt")]
    CooperativeFlat = 2
}