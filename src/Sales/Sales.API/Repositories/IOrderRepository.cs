using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Features.OrderManagement.Repositories;

public interface IOrderRepository : IRepository<Order, string>
{
    Task<Order?> FindByNoAsync(int orderNo, CancellationToken cancellationToken = default);
}