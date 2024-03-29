namespace YourBrand.Sales.Features.OrderManagement.Domain.Entities;

public class Organization : AggregateRoot<string>, IAuditable
{
    public Organization(string id, string name)
        : base(id)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}