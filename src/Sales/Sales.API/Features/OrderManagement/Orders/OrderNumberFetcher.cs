using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public sealed class OrderNumberFetcher(SalesContext salesContext)
{
    public async Task<int> GetNextNumberAsync(string organizationId, CancellationToken cancellationToken)
    {
        int orderNo;

        try
        {
            orderNo = (await salesContext.Orders
                .IgnoreQueryFilters()
                .InOrganization(organizationId)
                .MaxAsync(x => x.OrderNo.GetValueOrDefault(), cancellationToken: cancellationToken)) + 1;
        }
        catch (InvalidOperationException e)
        {
            orderNo = 1; // Order start number
        }

        return orderNo;
    }
}