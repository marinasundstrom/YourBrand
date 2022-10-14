
using YourBrand.Inventory.Domain;

using MediatR;
using YourBrand.Inventory.Application.Items;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record CreateItem(string Id, string Name) : IRequest<ItemDto>
{
    public class Handler : IRequestHandler<CreateItem, ItemDto>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(CreateItem request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (item is not null) throw new Exception();

            item = new Domain.Entities.Item(request.Id, request.Name, 0);;

            _context.Items.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            item = await _context
               .Items
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}
