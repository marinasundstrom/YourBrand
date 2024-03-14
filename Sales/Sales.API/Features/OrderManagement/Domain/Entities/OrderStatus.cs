using System.Collections.Generic;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

public class OrderStatus : Entity<int>, IAuditable
{
    protected OrderStatus()
    {
    }

    public OrderStatus(string name, string handle, string? description)
    {
        Name = name;
        Handle = handle;
        Description = description;
    }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public string? Description { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}