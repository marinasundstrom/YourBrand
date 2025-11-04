
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Commands;

public record UpdateItem(string Id, string NewId, string Name, string GroupId, string Unit) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<UpdateItem>
    {
        public async Task Handle(UpdateItem request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Rename(request.Name);
            item.ChangeGroup(request.GroupId);
            item.ChangeUnit(request.Unit);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}