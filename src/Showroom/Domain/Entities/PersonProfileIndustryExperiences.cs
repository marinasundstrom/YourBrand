using YourBrand.Domain;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileIndustryExperiences : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public Industry Industry { get; set; } = null!;

    public int IndustryId { get; set; }

    public int Years { get; set; }
    
    public string? DeletedById { get; set; }

    public DateTimeOffset? Deleted { get; set; }
}