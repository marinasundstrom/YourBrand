using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Application.Customers.Queries;

public record GetCustomerBySSN(string SSN) : IRequest<CustomerDto?>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetCustomerBySSN, CustomerDto?>
    {
        public async Task<CustomerDto?> Handle(GetCustomerBySSN request, CancellationToken cancellationToken)
        {
            var person = await context.Customers
                .Include(i => ((Person)i).Addresses)
                .Include(i => ((Organization)i).Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => ((Person)x).Ssn == request.SSN, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}