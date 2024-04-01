using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Addresses.Queries;

public record GetAddress(string AddressId) : IRequest<AddressDto?>
{
    public class Handler : IRequestHandler<GetAddress, AddressDto?>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<AddressDto?> Handle(GetAddress request, CancellationToken cancellationToken)
        {
            var person = await _context.Addresses
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AddressId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}