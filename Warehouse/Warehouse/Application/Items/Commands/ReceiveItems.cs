
using YourBrand.Warehouse.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Application.Items.Commands;

public record ReceiveItems(string Id, int Quantity) : IRequest
{
    public class Handler : IRequestHandler<ReceiveItems>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ReceiveItems request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Receive(request.Quantity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
