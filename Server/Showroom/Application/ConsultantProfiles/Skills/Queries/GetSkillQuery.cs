using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Application.ConsultantProfiles.Skills.Queries;

public class GetSkillQuery : IRequest<ConsultantProfileSkillDto?>
{
    public GetSkillQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

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
               .FirstAsync(c => c.SkillId == request.Id, cancellationToken);

            if (consultantProfileSkill is null)
            {
                return null;
            }

            return consultantProfileSkill.ToDto();
        }
    }
}
