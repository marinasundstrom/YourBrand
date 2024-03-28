using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Persons.Profile.Experiences
{
    public enum EmploymentType 
    {
        [Display(Name = "Contract")]
        Contract,

        [Display(Name = "Full-time")]
        FullTime,

        [Display(Name = "Part-time")]
        PartTime
    }
}

