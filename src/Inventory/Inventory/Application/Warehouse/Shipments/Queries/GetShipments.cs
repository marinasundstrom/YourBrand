using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Domain;

using ModelSortDirection = YourBrand.Inventory.Application.Common.Models.SortDirection;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Queries;

public record GetShipments(int Page, int PageSize, string WarehouseId, string? OrderNo = null, string? SearchString = null, string? SortBy = null, ModelSortDirection? SortDirection = null) : IRequest<ItemsResult<ShipmentDto>>;

public class GetShipmentsHandler(IInventoryContext context) : IRequestHandler<GetShipments, ItemsResult<ShipmentDto>>
{
    public async Task<ItemsResult<ShipmentDto>> Handle(GetShipments request, CancellationToken cancellationToken)
    {
        var query = context.Shipments
            .AsNoTracking()
            .Where(x => x.WarehouseId == request.WarehouseId);

        if (!string.IsNullOrWhiteSpace(request.OrderNo))
        {
            query = query.Where(x => x.OrderNo == request.OrderNo);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            var search = request.SearchString;

            query = query.Where(x =>
                x.OrderNo.Contains(search) ||
                x.Destination.FirstName.Contains(search) ||
                x.Destination.LastName.Contains(search) ||
                (x.Destination.CareOf != null && x.Destination.CareOf.Contains(search)) ||
                x.Destination.Address.Street.Contains(search) ||
                (x.Destination.Address.AddressLine2 != null && x.Destination.Address.AddressLine2.Contains(search)) ||
                x.Destination.Address.City.Contains(search) ||
                (x.Destination.Address.StateOrProvince != null && x.Destination.Address.StateOrProvince.Contains(search)) ||
                (x.Destination.Address.CareOf != null && x.Destination.Address.CareOf.Contains(search)) ||
                x.Destination.Address.PostalCode.Contains(search) ||
                x.Destination.Address.Country.Contains(search));
        }

        var sortDirectionIsDesc = request.SortDirection switch
        {
            ModelSortDirection.Asc => false,
            ModelSortDirection.Desc => true,
            _ => true
        };

        query = request.SortBy switch
        {
            "orderNo" when sortDirectionIsDesc => query.OrderByDescending(x => x.OrderNo),
            "orderNo" => query.OrderBy(x => x.OrderNo),
            "destination" when sortDirectionIsDesc => query
                .OrderByDescending(x => x.Destination.LastName)
                .ThenByDescending(x => x.Destination.FirstName),
            "destination" => query
                .OrderBy(x => x.Destination.LastName)
                .ThenBy(x => x.Destination.FirstName),
            "shippedAt" when sortDirectionIsDesc => query.OrderByDescending(x => x.ShippedAt),
            "shippedAt" => query.OrderBy(x => x.ShippedAt),
            _ when sortDirectionIsDesc => query.OrderByDescending(x => x.Created),
            _ => query.OrderBy(x => x.Created)
        };

        var total = await query.CountAsync(cancellationToken);

        var shipments = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
                    .ThenInclude(x => x.Item)
                        .ThenInclude(x => x.Group)
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
                    .ThenInclude(x => x.Warehouse)
                        .ThenInclude(x => x.Site)
            .ToListAsync(cancellationToken);

        return new ItemsResult<ShipmentDto>(shipments.Select(x => x.ToDto()), total);
    }
}
