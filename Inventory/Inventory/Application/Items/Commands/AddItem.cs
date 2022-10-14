
using YourBrand.Inventory.Domain;

using MediatR;
using YourBrand.Inventory.Application.Items;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record AddItem(string Id, string Name, string GroupId, string Unit) : IRequest<ItemDto>
{
    public class Handler : IRequestHandler<AddItem, ItemDto>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(AddItem request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (item is not null) throw new Exception();

            item = new Domain.Entities.Item(request.Id, request.Name, request.GroupId, request.Unit);

            _context.Items.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            item = await _context.Items       
               .Include(x => x.Group)
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}
