using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Commands;

public record DeleteAddress(string AddressId) : IRequest
{
    public class Handler(ICustomersContext context) : IRequestHandler<DeleteAddress>
    {
        public async Task Handle(DeleteAddress request, CancellationToken cancellationToken)
        {
            var address = await context.Addresses
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AddressId, cancellationToken);

            if (address is null)
            {
                throw new Exception();
            }

            context.Addresses.Remove(address);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}