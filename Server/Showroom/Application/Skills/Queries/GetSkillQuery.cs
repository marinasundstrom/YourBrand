using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Skills.Queries;

public class GetSkillQuery : IRequest<SkillDto?>
{
    public GetSkillQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

    class GetSkillQueryHandler : IRequestHandler<GetSkillQuery, SkillDto?>
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

        public async Task<SkillDto?> Handle(GetSkillQuery request, CancellationToken cancellationToken)
        {
            var skill = await _context
               .Skills
               .Include(x => x.Area)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id, cancellationToken);

            if (skill is null)
            {
                return null;
            }

            return skill.ToDto();
        }
    }
}
