using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Groups.Queries;

public record GetItemGroup(string ItemGroupId) : IRequest<ItemGroupDto?>
{
    public class Handler(IInventoryContext context) : IRequestHandler<GetItemGroup, ItemGroupDto?>
    {
        public async Task<ItemGroupDto?> Handle(GetItemGroup request, CancellationToken cancellationToken)
        {
            var person = await context.ItemGroups
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemGroupId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}