using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain.Specifications;
using YourBrand.Identity;

namespace YourBrand.Meetings.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    readonly ApplicationDbContext context;
    readonly DbSet<User> dbSet;

    public UserRepository(ApplicationDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<User>();
    }

    public IQueryable<User> GetAll()
    {
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