using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Queries;

public record GetPersonProfileQuery(string Id) : IRequest<PersonProfileDto>
{
    class GetPersonProfileQueryHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<GetPersonProfileQuery, PersonProfileDto?>
    {
        public async Task<PersonProfileDto?> Handle(GetPersonProfileQuery request, CancellationToken cancellationToken)
        {
            var personProfile = await context
               .PersonProfiles
               .Include(x => x.Industry)
               .Include(x => x.Organization)
               .Include(c => c.CompetenceArea)
               //.Include(c => c.Manager)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (personProfile is null)
            {
                return null;
            }

            return personProfile.ToDto(urlHelper);
        }
    }
}