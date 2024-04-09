using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Application.Customers.Queries;

public record GetCustomer(int CustomerId) : IRequest<CustomerDto?>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetCustomer, CustomerDto?>
    {
        public async Task<CustomerDto?> Handle(GetCustomer request, CancellationToken cancellationToken)
        {
            var person = await context.Customers
                .Include(i => ((Person)i).Addresses)
                .Include(i => ((Organization)i).Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}