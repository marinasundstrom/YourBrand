﻿using MediatR;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Persons.Commands;

public record DeletePerson(string PersonId) : IRequest
{
    public class Handler(ICustomersContext context) : IRequestHandler<DeletePerson>
    {
        public async Task Handle(DeletePerson request, CancellationToken cancellationToken)
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
        }
    }
}