using YourBrand.Identity;

namespace YourBrand.Customers.Domain.Entities;

public interface IAuditable
{
    UserId? CreatedById { get; set; }
    DateTimeOffset Created { get; set; }

    UserId? LastModifiedById { get; set; }
    DateTimeOffset? LastModified { get; set; }
}