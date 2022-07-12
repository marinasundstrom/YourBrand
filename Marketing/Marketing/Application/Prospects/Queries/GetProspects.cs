using YourBrand.Marketing.Domain;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Application.Prospects;

namespace YourBrand.Marketing.Application.Prospects.Queries;

public record GetProspects(int Page = 1, int PageSize = 10) : IRequest<ItemsResult<ProspectDto>>
{
    public class Handler : IRequestHandler<GetProspects, ItemsResult<ProspectDto>>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProspectDto>> Handle(GetProspects request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Prospects
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

            return new ItemsResult<ProspectDto>(
                items.Select(invoice => invoice.ToDto()),
                totalItems);
        }
    }
}