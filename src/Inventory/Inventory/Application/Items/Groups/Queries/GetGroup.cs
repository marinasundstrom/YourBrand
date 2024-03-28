using YourBrand.Inventory.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Warehouses.Items;

namespace YourBrand.Inventory.Application.Items.Groups.Queries;

public record GetItemGroup(string ItemGroupId) : IRequest<ItemGroupDto?>
{
    public class Handler : IRequestHandler<GetItemGroup, ItemGroupDto?>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<ItemGroupDto?> Handle(GetItemGroup request, CancellationToken cancellationToken)
        {
            var person = await _context.ItemGroups
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemGroupId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}