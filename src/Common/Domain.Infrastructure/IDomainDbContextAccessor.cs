using Microsoft.EntityFrameworkCore;

namespace YourBrand.Domain.Infrastructure;

public interface IDomainDbContextAccessor 
{
    DbContext DbContext{ get; }
}
