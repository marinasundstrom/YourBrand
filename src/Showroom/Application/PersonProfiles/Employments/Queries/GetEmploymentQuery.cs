using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments.Queries;

public record GetEmploymentQuery(string PersonProfileId, string Id) : IRequest<EmploymentDto>
{
    sealed class GetPersonProfileQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetEmploymentQuery, EmploymentDto?>
    {
        public async Task<EmploymentDto?> Handle(GetEmploymentQuery request, CancellationToken cancellationToken)
        {
            var employment = await context
               .Employments
                .Include(x => x.Employer)
                .ThenInclude(x => x.Industry)
               .Include(x => x.Skills)
               .ThenInclude(x => x.PersonProfileSkill)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (employment is null)
            {
                return null;
            }

            return employment.ToDto();
        }
    }
}