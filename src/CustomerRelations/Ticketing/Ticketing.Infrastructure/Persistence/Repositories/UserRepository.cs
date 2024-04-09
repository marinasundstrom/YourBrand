using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Specifications;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    readonly DbSet<User> dbSet = context.Set<User>();

    public IQueryable<User> GetAll()
    {
        //return dbSet.Where(new UsersWithStatus(UserStatus.Completed).Or(new UsersWithStatus(UserStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
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