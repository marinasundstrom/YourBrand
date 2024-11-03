using YourBrand.Auditability;
using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Entities;

public class OrganizationUser : AggregateRoot<string>, IAuditableEntity<string, User>
{
    public OrganizationUser()
        : base(Guid.NewGuid().ToString())
    {

    }

    public OrganizationUser(string id)
        : base(id)
    {

    }

    public OrganizationId OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public UserId UserId { get; set; }

    public User User { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}