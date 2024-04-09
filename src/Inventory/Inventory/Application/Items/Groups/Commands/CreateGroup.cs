
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Groups.Commands;

public record CreateItemGroup(string Name) : IRequest<ItemGroupDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<CreateItemGroup, ItemGroupDto>
    {
        public async Task<ItemGroupDto> Handle(CreateItemGroup request, CancellationToken cancellationToken)
        {
            var item = new Domain.Entities.ItemGroup(request.Name);

            context.ItemGroups.Add(item);

            await context.SaveChangesAsync(cancellationToken);

            item = await context.ItemGroups
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}