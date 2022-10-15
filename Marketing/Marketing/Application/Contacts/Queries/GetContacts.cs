using YourBrand.Marketing.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Application.Contacts;

namespace YourBrand.Marketing.Application.Contacts.Queries;

public record GetContacts(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ContactDto>>
{
    public class Handler : IRequestHandler<GetContacts, ItemsResult<ContactDto>>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ContactDto>> Handle(GetContacts request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Contacts
                .AsSplitQuery()
                .AsNoTracking()
                .AsQueryable();

            if (request.SearchString is not null)
            {
                query = query.Where(o => 
                    o.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || o.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || o.Ssn.ToLower().Contains(request.SearchString.ToLower()));
            }

            int totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Marketing.Application.SortDirection.Descending : Marketing.Application.SortDirection.Ascending);
            }
            else 
            {
                query = query.OrderBy(x => x.Id);
            }

            query = query
                .Include(i => i.Campaign)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<ContactDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}