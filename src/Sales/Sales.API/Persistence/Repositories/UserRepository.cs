using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Specifications;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Persistence.Repositories.Mocks;

public sealed class UserRepository(SalesContext context) : IUserRepository
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

public static class IncludeExtensions
{
    public static IQueryable<Order> IncludeAll(this IQueryable<Order> source)
    {
        return source
            .Include(i => i.Parent)
            .Include(i => i.Type)
            .Include(i => i.Status)
            .Include(i => i.Subscription)
            .ThenInclude(i => i.Type)
            .Include(i => i.Subscription)
            .ThenInclude(i => i.Status)
            .Include(i => i.Subscription)
            .ThenInclude(i => i.Plan)
            .Include(i => i.Items)
            .ThenInclude(i => i.SubscriptionPlan)
            .Include(i => i.Items)
            .ThenInclude(i => i.Subscription)
            .ThenInclude(i => i.Plan)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy);
    }
}