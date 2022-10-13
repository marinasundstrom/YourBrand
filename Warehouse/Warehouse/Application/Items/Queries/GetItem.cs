using YourBrand.Warehouse.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Warehouse.Application.Items;

namespace YourBrand.Warehouse.Application.Items.Queries;

public record GetItem(string ItemId) : IRequest<ItemDto?>
{
    public class Handler : IRequestHandler<GetItem, ItemDto?>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<ItemDto?> Handle(GetItem request, CancellationToken cancellationToken)
        {
            var person = await _context.Items
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}