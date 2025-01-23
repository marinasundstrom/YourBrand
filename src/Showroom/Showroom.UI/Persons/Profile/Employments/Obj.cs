using YourBrand.Showroom.Client;
using System.ComponentModel.DataAnnotations;

namespace YourBrand.Showroom.Persons.Profile.Employments;

public record Obj(Employment employment, bool isSub);

public enum ExperienceType 
{
    [DisplayAttribute(Name = "Employment")]
    Employment,

    [DisplayAttribute(Name = "Assignment")]
    Assignment,

    [DisplayAttribute(Name = "Project")]
    Project
}