using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Addresses.Commands;

public record DeleteAddress(string AddressId) : IRequest
{
    public class Handler(IMarketingContext context) : IRequestHandler<DeleteAddress>
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