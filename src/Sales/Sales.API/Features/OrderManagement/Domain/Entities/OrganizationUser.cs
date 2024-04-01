namespace YourBrand.Sales.Features.OrderManagement.Domain.Entities;

public class OrganizationUser : AggregateRoot<string>, IAuditable
{
    public OrganizationUser()
        : base(Guid.NewGuid().ToString())
    {
        
    }

    public OrganizationUser(string id)
        : base(id)
    {
        Id = id;
    }

    public string OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}