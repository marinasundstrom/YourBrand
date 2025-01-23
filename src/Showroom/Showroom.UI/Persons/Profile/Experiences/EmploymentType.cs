using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Persons.Profile.Experiences;

public enum EmploymentType
{

    [Display(Name = "Full-time")]
    FullTime,

    [Display(Name = "Part-time")]
    PartTime,

    [Display(Name = "Temporary")]
    Temporary,

    [Display(Name = "Contract")]
    Contract
}