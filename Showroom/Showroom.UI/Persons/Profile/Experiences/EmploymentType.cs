using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Persons.Profile.Experiences
{
    public enum EmploymentType 
    {
        [DisplayAttribute(Name = "Contract")]
        Contract,

        [DisplayAttribute(Name = "Full-time")]
        FullTime
    }
}

