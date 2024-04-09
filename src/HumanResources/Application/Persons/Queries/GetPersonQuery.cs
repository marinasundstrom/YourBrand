
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Application.Persons.Queries;

public record GetPersonQuery(string PersonId) : IRequest<PersonDto>
{
    public class GetPersonQueryHandler(IApplicationDbContext context) : IRequestHandler<GetPersonQuery, PersonDto>
    {
        public async Task<PersonDto> Handle(GetPersonQuery request, CancellationToken cancellationToken)
        {
            var person = await context.Persons
                .Include(u => u.Roles)
                .Include(u => u.Organization)
                .Include(u => u.Department)
                .Include(u => u.ReportsTo)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                return null!;
            }

            return person.ToDto();
        }
    }
}