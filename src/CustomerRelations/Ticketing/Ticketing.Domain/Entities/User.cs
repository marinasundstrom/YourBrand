using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Domain.Entities;

public class User : AggregateRoot<UserId>, IAuditable, IHasTenant
{
    public User(UserId id, string name, string email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public TenantId TenantId { get; set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}