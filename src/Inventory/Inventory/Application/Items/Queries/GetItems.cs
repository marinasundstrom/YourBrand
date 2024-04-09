using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Items.Queries;

public record GetItems(int Page = 0, int PageSize = 10, string? GroupId = null, string? WarehouseId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ItemDto>>
{
    public class Handler(IInventoryContext context) : IRequestHandler<GetItems, ItemsResult<ItemDto>>
    {
        public async Task<ItemsResult<ItemDto>> Handle(GetItems request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            IQueryable<Item> result = context
                      .Items
                      .AsNoTracking()
                      .AsQueryable();

            if (request.WarehouseId is not null)
            {
                result = result.Where(o => o.WarehouseItems.Any(x => x.WarehouseId == request.WarehouseId));
            }

            if (request.GroupId is not null)
            {
                result = result.Where(o => o.GroupId == request.GroupId);
            }

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Inventory.Application.SortDirection.Descending : Inventory.Application.SortDirection.Ascending);
            }
            else
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Include(x => x.Group)
                .Include(x => x.WarehouseItems.Where(x => request.WarehouseId == null || x.WarehouseId == request.WarehouseId))
                .AsSingleQuery()
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto()).ToList();

            return new ItemsResult<ItemDto>(
                items.Select(item => item.ToDto()),
                totalCount);
        }
    }
}