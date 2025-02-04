
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class TaskType : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    public TaskType(string name, string? description) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public Organization Organization { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;



    public Project? Project { get; set; }

    public bool ExcludeHours { get; set; }

    public List<Task> Tasks { get; set; } = new List<Task>();

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}