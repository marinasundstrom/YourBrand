using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Queries;

public record GetSkillAreaQuery(string Id) : IRequest<SkillAreaDto?>
{
    sealed class GetSkillAreaQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetSkillAreaQuery, SkillAreaDto?>
    {
        public async Task<SkillAreaDto?> Handle(GetSkillAreaQuery request, CancellationToken cancellationToken)
        {
            var skillArea = await context
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