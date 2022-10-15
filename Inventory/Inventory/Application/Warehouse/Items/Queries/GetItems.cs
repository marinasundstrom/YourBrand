using YourBrand.Inventory.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Warehouses.Items;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Warehouses.Items.Queries;

public record GetWarehouseItems(int Page = 0, int PageSize = 10, string? WarehouseId = null, string? ItemId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<WarehouseItemDto>>
{
    public class Handler : IRequestHandler<GetWarehouseItems, ItemsResult<WarehouseItemDto>>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<WarehouseItemDto>> Handle(GetWarehouseItems request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

          IQueryable<WarehouseItem> result = _context
                    .WarehouseItems
                    .AsNoTracking()
                    .AsQueryable();

            if (request.WarehouseId is not null)
            {
                result = result.Where(o => o.WarehouseId == request.WarehouseId);
            }

            if (request.ItemId is not null)
            {
                result = result.Where(o => o.ItemId == request.ItemId);
            }

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.Item.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Inventory.Application.SortDirection.Descending : Inventory.Application.SortDirection.Ascending);
            }
            else 
            {
                // TODO: Revisit this. Should this be the default?

                result = result
                    .OrderByDescending(x => x.Warehouse.Site)
                    .ThenByDescending(x => x.Warehouse);
            }

            var items = await result
                .Include(x => x.Item)     
                .ThenInclude(x => x.Group)
                .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto()).ToList();

            return new ItemsResult<WarehouseItemDto>(
                items.Select(item => item.ToDto()),
                totalCount);
        }
    }
}