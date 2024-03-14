using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Specifications;

namespace YourBrand.Sales.API.Features.OrderManagement.Repositories;

public interface IOrderRepository : IRepository<Order, string>
{
    Task<Order?> FindByNoAsync(int orderNo, CancellationToken cancellationToken = default);
}