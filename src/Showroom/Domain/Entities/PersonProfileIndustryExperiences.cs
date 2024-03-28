using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileIndustryExperiences : Entity
{
    public string Id { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public Industry Industry { get; set; } = null!;

    public int IndustryId { get; set; }

    public int Years { get; set; }
}