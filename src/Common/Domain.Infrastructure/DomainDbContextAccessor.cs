using Microsoft.EntityFrameworkCore;

namespace YourBrand.Domain.Infrastructure;

public class DomainDbContextAccessor(DbContext dbContext) : IDomainDbContextAccessor
{
    public DbContext DbContext { get; } = dbContext;
}
