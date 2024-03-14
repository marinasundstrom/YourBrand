using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Specifications;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;
using YourBrand.Sales.API.Persistence;

namespace YourBrand.Sales.API.Persistence.Repositories.Mocks;

public sealed class OrderRepository : IOrderRepository
{
    readonly SalesContext context;
    readonly DbSet<Order> dbSet;

    public OrderRepository(SalesContext context)
    {
        this.context = context;
        this.dbSet = context.Set<Order>();
    }

    public IQueryable<Order> GetAll()
    {
        //return dbSet.Where(new OrdersWithStatus(OrderStatus.Completed).Or(new OrdersWithStatus(OrderStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<Order?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Include(i => i.Status)
            .Include(i => i.Items)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<Order?> FindByNoAsync(int orderNo, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Include(i => i.Status)
            .Include(i => i.Items)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .FirstOrDefaultAsync(x => x.OrderNo.Equals(orderNo), cancellationToken);
    }

    public IQueryable<Order> GetAll(ISpecification<Order> specification)
    {
        return dbSet
            .Include(i => i.Status)
            .Include(i => i.Items)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
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