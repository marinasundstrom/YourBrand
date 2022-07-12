using YourBrand.Marketing.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Application.Prospects;

namespace YourBrand.Marketing.Application.Prospects.Queries;

public record GetProspect(string ProspectId) : IRequest<ProspectDto?>
{
    public class Handler : IRequestHandler<GetProspect, ProspectDto?>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ProspectDto?> Handle(GetProspect request, CancellationToken cancellationToken)
        {
            /*
            var person = await _context.Prospects
                .Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProspectId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
            */

            return null;
        }
    }
}