using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Application.Customers.Queries;

public record GetCustomers(int Page = 1, int PageSize = 10, string? SearchString = null) : IRequest<ItemsResult<CustomerDto>>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetCustomers, ItemsResult<CustomerDto>>
    {
        public async Task<ItemsResult<CustomerDto>> Handle(GetCustomers request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Customers
                .AsSplitQuery()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .AsQueryable();

            if (request.SearchString is not null)
            {
                query = query.Where(x => x.Id.ToString().Contains(request.SearchString)
                    || x.Name.ToLower().Contains(request.SearchString.ToLower())
                    || x.Phone!.ToLower().Contains(request.SearchString.ToLower())
                    || x.PhoneMobile.ToLower().Contains(request.SearchString.ToLower())
                    || x.Email.ToLower().Contains(request.SearchString.ToLower())
                    || (x as Domain.Entities.Organization)!.OrganizationNo.ToLower().Contains(request.SearchString.ToLower())
                    || (x as Domain.Entities.Person)!.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || (x as Domain.Entities.Person)!.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || (x as Domain.Entities.Person)!.Ssn.ToLower().Contains(request.SearchString.ToLower()));
            }

            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Include(i => ((Person)i).Addresses)
                .Include(i => ((Organization)i).Addresses)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<CustomerDto>(
                items.Select(c => c.ToDto()),
                totalItems);
        }
    }
}