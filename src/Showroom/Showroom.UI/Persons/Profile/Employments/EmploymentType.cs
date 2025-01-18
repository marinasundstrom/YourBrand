using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Persons.Profile.Employments;

public enum EmploymentType
{
    [Display(Name = "Contract")]
    Contract,

    [Display(Name = "Full-time")]
    FullTime,

    [Display(Name = "Part-time")]
    PartTime
}