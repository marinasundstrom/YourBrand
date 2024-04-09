using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Addresses.Queries;

public record GetAddresses(int Page = 1, int PageSize = 10) : IRequest<ItemsResult<AddressDto>>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetAddresses, ItemsResult<AddressDto>>
    {
        public async Task<ItemsResult<AddressDto>> Handle(GetAddresses request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Addresses
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                //.Include(i => i.Addresses)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<AddressDto>(
                items.Select(address => address.ToDto()),
                totalItems);
        }
    }
}