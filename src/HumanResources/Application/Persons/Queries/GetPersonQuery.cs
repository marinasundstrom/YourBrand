
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Application.Persons.Queries;

public record GetPersonQuery(string PersonId) : IRequest<PersonDto>
{
    public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDto>
    {
        private readonly IApplicationDbContext _context;

        public GetPersonQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonDto> Handle(GetPersonQuery request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
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