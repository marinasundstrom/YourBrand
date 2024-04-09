using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Addresses.Queries;

public record GetAddress(string AddressId) : IRequest<AddressDto?>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetAddress, AddressDto?>
    {
        public async Task<AddressDto?> Handle(GetAddress request, CancellationToken cancellationToken)
        {
            var person = await context.Addresses
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AddressId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}