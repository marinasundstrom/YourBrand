using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;


public class Project : AuditableEntity<string>, IHasTenant
{
    public Project()
        : base(Guid.NewGuid().ToString())
    {

    }
    public TenantId TenantId { get; set; } = null!;

    public string Name { get; set; }

    public Company? Company { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public List<PersonProfileSkill> Skills { get; set; } = new List<PersonProfileSkill>();
}