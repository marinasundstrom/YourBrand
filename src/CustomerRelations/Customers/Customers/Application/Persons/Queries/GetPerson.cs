using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;

namespace YourBrand.Customers.Application.Persons.Queries;

public record GetPerson(int Id) : IRequest<PersonDto?>
{
    public class Handler(ICustomersContext context) : IRequestHandler<GetPerson, PersonDto?>
    {
        public async Task<PersonDto?> Handle(GetPerson request, CancellationToken cancellationToken)
        {
            var person = await context.Persons
                .Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}