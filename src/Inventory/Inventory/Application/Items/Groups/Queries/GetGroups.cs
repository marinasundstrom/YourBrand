using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Items.Groups.Queries;

public record GetItemGroups(int Page = 0, int PageSize = 10, string? WarehouseId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ItemGroupDto>>
{
    public class Handler(IInventoryContext context) : IRequestHandler<GetItemGroups, ItemsResult<ItemGroupDto>>
    {
        public async Task<ItemsResult<ItemGroupDto>> Handle(GetItemGroups request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            IQueryable<ItemGroup> result = context.ItemGroups
                      .AsNoTracking()
                      .AsQueryable();

            /*
            if (request.WarehouseId is not null)
            {
                result = result.Where(o => o.WarehouseId == request.WarehouseId);
            }
            */

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
                result = result.OrderBy(x => x.Id);
            }

            var items = await result
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto()).ToList();

            return new ItemsResult<ItemGroupDto>(
                items.Select(item => item.ToDto()),
                totalCount);
        }
    }
}