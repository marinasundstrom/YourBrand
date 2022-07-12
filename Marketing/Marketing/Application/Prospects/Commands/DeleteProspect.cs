using YourBrand.Marketing.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Application.Prospects.Commands;

public record DeleteProspect(string ProspectId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProspect>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProspect request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.Prospects
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProspectId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.Prospects.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */

            return Unit.Value;
        }
    }
}