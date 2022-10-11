using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

public record GetSkillQuery(string Id) : IRequest<PersonProfileSkillDto?>
{

    class GetSkillQueryHandler : IRequestHandler<GetSkillQuery, PersonProfileSkillDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSkillQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<PersonProfileSkillDto?> Handle(GetSkillQuery request, CancellationToken cancellationToken)
        {
            var personProfileSkill = await _context
               .PersonProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)         
               .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.SkillId == request.Id, cancellationToken);

            if (personProfileSkill is null)
            {
                return null;
            }

            return personProfileSkill.ToDto();
        }
    }
}
