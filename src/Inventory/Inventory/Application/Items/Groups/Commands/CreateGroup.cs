
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Groups.Commands;

public record CreateItemGroup(string Name) : IRequest<ItemGroupDto>
{
    public class Handler : IRequestHandler<CreateItemGroup, ItemGroupDto>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<ItemGroupDto> Handle(CreateItemGroup request, CancellationToken cancellationToken)
        {
            var item = new Domain.Entities.ItemGroup(request.Name);

            _context.ItemGroups.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            item = await _context.ItemGroups
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}