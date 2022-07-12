using YourBrand.Customers.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Application.Persons.Commands;

public record DeletePerson(string PersonId) : IRequest
{
    public class Handler : IRequestHandler<DeletePerson>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeletePerson request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.Persons
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.Persons.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */

            return Unit.Value;
        }
    }
}