
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Groups.Commands;

public record UpdateItemGroup(string Id, string Name) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<UpdateItemGroup>
    {
        public async Task Handle(UpdateItemGroup request, CancellationToken cancellationToken)
        {
            var item = await context.ItemGroups.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}