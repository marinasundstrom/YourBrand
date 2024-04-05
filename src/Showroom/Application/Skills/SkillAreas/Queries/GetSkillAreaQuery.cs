using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Queries;

public record GetSkillAreaQuery(string Id) : IRequest<SkillAreaDto?>
{
    class GetSkillAreaQueryHandler : IRequestHandler<GetSkillAreaQuery, SkillAreaDto?>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetSkillAreaQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<SkillAreaDto?> Handle(GetSkillAreaQuery request, CancellationToken cancellationToken)
        {
            var skillArea = await _context
               .SkillAreas
                .Include(x => x.Industry)
                .Include(x => x.Skills)
               //.AsNoTracking()
               .FirstAsync(c => c.Id == request.Id, cancellationToken);

            if (skillArea is null)
            {
                return null;
            }

            return skillArea.ToDto();
        }
    }
}