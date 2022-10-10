using YourBrand.Customers.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Customers.Application.Organizations;

namespace YourBrand.Customers.Application.Organizations.Queries;

public record GetOrganizations(int Page = 1, int PageSize = 10, string? SearchString = null) : IRequest<ItemsResult<OrganizationDto>>
{
    public class Handler : IRequestHandler<GetOrganizations, ItemsResult<OrganizationDto>>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<OrganizationDto>> Handle(GetOrganizations request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Organizations
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if(request.SearchString is not null) 
            {
                query = query.Where(x => x.Id.ToString().Contains(request.SearchString) 
                    || x.Name.ToLower().Contains(request.SearchString.ToLower())
                    || (x as Domain.Entities.Organization)!.OrganizationNo.ToLower().Contains(request.SearchString.ToLower()));
            }          

            int totalItems = await query.CountAsync(cancellationToken);

            query = query         
                .Include(i => i.Addresses)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<OrganizationDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}