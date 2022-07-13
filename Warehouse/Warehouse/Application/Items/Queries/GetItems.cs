using YourBrand.Warehouse.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Warehouse.Application.Items;

namespace YourBrand.Warehouse.Application.Items.Queries;

public record GetItems(int Page = 1, int PageSize = 10) : IRequest<ItemsResult<ItemDto>>
{
    public class Handler : IRequestHandler<GetItems, ItemsResult<ItemDto>>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ItemDto>> Handle(GetItems request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Items
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            int totalItems = await query.CountAsync(cancellationToken);

            query = query         
                //.Include(i => i.Addresses)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<ItemDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}