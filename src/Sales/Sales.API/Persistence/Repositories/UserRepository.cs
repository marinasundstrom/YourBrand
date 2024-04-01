using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Specifications;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Persistence.Repositories.Mocks;

public sealed class UserRepository : IUserRepository
{
    readonly SalesContext context;
    readonly DbSet<User> dbSet;

    public UserRepository(SalesContext context)
    {
        this.context = context;
        this.dbSet = context.Set<User>();
    }

    public IQueryable<User> GetAll()
    {
        //return dbSet.Where(new UsersWithStatus(UserStatus.Completed).Or(new UsersWithStatus(UserStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<User> GetAll(ISpecification<User> specification)
    {
        return dbSet.Where(specification.Criteria);
    }

    public void Add(User user)
    {
        dbSet.Add(user);
    }

    public void Remove(User user)
    {
        dbSet.Remove(user);
    }
}
