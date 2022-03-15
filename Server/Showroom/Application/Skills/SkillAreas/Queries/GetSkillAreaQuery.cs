using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Application.Skills.SkillAreas.Queries;

public class GetSkillAreaQuery : IRequest<SkillAreaDto?>
{
    public GetSkillAreaQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }

    class GetSkillAreaQueryHandler : IRequestHandler<GetSkillAreaQuery, SkillAreaDto?>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSkillAreaQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<SkillAreaDto?> Handle(GetSkillAreaQuery request, CancellationToken cancellationToken)
        {
            var skillArea = await _context
               .SkillAreas
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id, cancellationToken);

            if (skillArea is null)
            {
                return null;
            }

            return skillArea.ToDto();
        }
    }
}
