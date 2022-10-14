using YourBrand.Inventory.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Items;

namespace YourBrand.Inventory.Application.Items.Queries;

public record GetItem(string ItemId) : IRequest<ItemDto?>
{
    public class Handler : IRequestHandler<GetItem, ItemDto?>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
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