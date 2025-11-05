using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders.Queries;

public record GetTransferOrder(string Id) : IRequest<TransferOrderDto?>;

public class GetTransferOrderHandler(IInventoryContext context) : IRequestHandler<GetTransferOrder, TransferOrderDto?>
{
    public async Task<TransferOrderDto?> Handle(GetTransferOrder request, CancellationToken cancellationToken)
    {
        var transferOrder = await context.TransferOrders
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Include(x => x.SourceWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.DestinationWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .FirstOrDefaultAsync(cancellationToken);

        return transferOrder?.ToDto();
    }
}
