using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class Employment : AuditableEntity<string>, IHasTenant
{
    public Employment()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public Company Employer { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}