
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Commands;

public record UpdateItem(string Id, string NewId, string Name, string GroupId, string Unit) : IRequest
{
    public class Handler : IRequestHandler<UpdateItem>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateItem request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Name = request.Name;
            item.GroupId = request.GroupId;
            item.Unit = request.Unit;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}