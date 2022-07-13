
using YourBrand.Warehouse.Domain;

using MediatR;
using YourBrand.Warehouse.Application.Items;

namespace YourBrand.Warehouse.Application.Items.Commands;

public record CreateItem(string FirstName, string LastName, string SSN) : IRequest<ItemDto>
{
    public class Handler : IRequestHandler<CreateItem, ItemDto>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(CreateItem request, CancellationToken cancellationToken)
        {
            /*
            var person = new Domain.Entities.Item(request.FirstName, request.LastName, request.SSN);

            _context.Items.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            return person.ToDto();
            */

            return null!;
        }
    }
}
