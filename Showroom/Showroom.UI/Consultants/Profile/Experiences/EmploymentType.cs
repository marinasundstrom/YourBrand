using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Consultants.Profile.Experiences
{
    public enum EmploymentType 
    {
        [DisplayAttribute(Name = "Contract")]
        Contract,

        [DisplayAttribute(Name = "Full-time")]
        FullTime
    }
}

