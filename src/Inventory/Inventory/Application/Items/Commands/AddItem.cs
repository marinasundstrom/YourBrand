
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Application.Items.Commands;

public record AddItem(string Id, string Name, ItemTypeDto Type, string GroupId, string Unit) : IRequest<ItemDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<AddItem, ItemDto>
    {
        public async Task<ItemDto> Handle(AddItem request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (item is not null) throw new Exception();

            item = new Domain.Entities.Item(request.Id, request.Name, (ItemType)request.Type, "Foo", request.GroupId, request.Unit);

            context.Items.Add(item);

            await context.SaveChangesAsync(cancellationToken);

            item = await context.Items
               .Include(x => x.Group)
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}