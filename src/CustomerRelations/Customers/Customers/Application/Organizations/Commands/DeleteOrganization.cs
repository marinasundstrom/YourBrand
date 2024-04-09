using MediatR;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Commands;

public record DeleteOrganization(string OrganizationId) : IRequest
{
    public class Handler(ICustomersContext context) : IRequestHandler<DeleteOrganization>
    {
        public async Task Handle(DeleteOrganization request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.Organizations
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.Organizations.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */
        }
    }
}