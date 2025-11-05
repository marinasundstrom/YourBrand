using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Domain;

using ModelSortDirection = YourBrand.Inventory.Application.Common.Models.SortDirection;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders.Queries;

public record GetTransferOrders(
    int Page,
    int PageSize,
    string? SourceWarehouseId,
    string? DestinationWarehouseId,
    string? SearchString = null,
    string? SortBy = null,
    ModelSortDirection? SortDirection = null) : IRequest<ItemsResult<TransferOrderDto>>;

public class GetTransferOrdersHandler(IInventoryContext context) : IRequestHandler<GetTransferOrders, ItemsResult<TransferOrderDto>>
{
    public async Task<ItemsResult<TransferOrderDto>> Handle(GetTransferOrders request, CancellationToken cancellationToken)
    {
        var query = context.TransferOrders
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SourceWarehouseId))
        {
            query = query.Where(x => x.SourceWarehouseId == request.SourceWarehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.DestinationWarehouseId))
        {
            query = query.Where(x => x.DestinationWarehouseId == request.DestinationWarehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            var search = request.SearchString.Trim();

            query = query.Where(x =>
                x.SourceWarehouse.Name.Contains(search) ||
                x.DestinationWarehouse.Name.Contains(search) ||
                x.Lines.Any(l => l.Item.Name.Contains(search)));
        }

        var sortDesc = request.SortDirection switch
        {
            ModelSortDirection.Asc => false,
            ModelSortDirection.Desc => true,
            _ => true
        };

        query = request.SortBy switch
        {
            "created" when sortDesc => query.OrderByDescending(x => x.Created),
            "created" => query.OrderBy(x => x.Created),
            "completedAt" when sortDesc => query.OrderByDescending(x => x.CompletedAt),
            "completedAt" => query.OrderBy(x => x.CompletedAt),
            _ when sortDesc => query.OrderByDescending(x => x.Created),
            _ => query.OrderBy(x => x.Created)
        };

        var total = await query.CountAsync(cancellationToken);

        var transferOrders = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Include(x => x.SourceWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.DestinationWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .ToListAsync(cancellationToken);

        return new ItemsResult<TransferOrderDto>(transferOrders.Select(x => x.ToDto()), total);
    }
}
