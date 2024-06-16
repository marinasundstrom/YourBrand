using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public sealed class OrderNumberFetcher(IOrderRepository orderRepository)
{
    public async Task<int> GetNextNumberAsync(string organizationId, CancellationToken cancellationToken)
    {
        int orderNo;

        try
        {
            orderNo = (await orderRepository
                .GetAll()
                .InOrganization(organizationId)
                .MaxAsync(x => x.OrderNo, cancellationToken: cancellationToken)) + 1;
        }
        catch (InvalidOperationException e)
        {
            orderNo = 1; // Order start number
        }

        return orderNo;
    }
}