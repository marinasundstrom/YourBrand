using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Specifications;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Persistence.Repositories.Mocks;

public sealed class OrderRepository(SalesContext context) : IOrderRepository
{
    readonly DbSet<Order> dbSet = context.Set<Order>();

    public IQueryable<Order> GetAll()
    {
        //return dbSet.Where(new OrdersWithStatus(OrderStatus.Completed).Or(new OrdersWithStatus(OrderStatus.OnHold))).AsQueryable();

        return dbSet
            .IncludeAll()
            .AsQueryable();
    }

    public async Task<Order?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .IncludeAll()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<Order?> FindByNoAsync(int orderNo, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .IncludeAll()
            .FirstOrDefaultAsync(x => x.OrderNo.Equals(orderNo), cancellationToken);
    }

    public IQueryable<Order> GetAll(ISpecification<Order> specification)
    {
        return dbSet
            .IncludeAll()
            .Where(specification.Criteria);
    }

    public void Add(Order item)
    {
        dbSet.Add(item);
    }

    public void Remove(Order item)
    {
        dbSet.Remove(item);
    }
}