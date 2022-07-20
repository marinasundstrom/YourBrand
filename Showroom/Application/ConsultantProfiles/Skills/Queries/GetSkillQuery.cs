using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

public record GetSkillQuery(string Id) : IRequest<ConsultantProfileSkillDto?>
{

    class GetSkillQueryHandler : IRequestHandler<GetSkillQuery, ConsultantProfileSkillDto?>
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

        public async Task<ConsultantProfileSkillDto?> Handle(GetSkillQuery request, CancellationToken cancellationToken)
        {
            var consultantProfileSkill = await _context
               .ConsultantProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
               .AsNoTracking()
               .FirstAsync(c => c.SkillId == request.Id, cancellationToken);

            if (consultantProfileSkill is null)
            {
                return null;
            }

            return consultantProfileSkill.ToDto();
        }
    }
}
