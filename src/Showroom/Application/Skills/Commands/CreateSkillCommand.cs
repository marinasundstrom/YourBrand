using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.Commands;

public record CreateSkillCommand(string Name, string SkillAreaId) : IRequest<SkillDto>
{
    public class CreateSkillCommandHandler(IShowroomContext context) : IRequestHandler<CreateSkillCommand, SkillDto>
    {
        public async Task<SkillDto> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.Skills.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (skill is not null) throw new Exception();

            skill = new Domain.Entities.Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Slug = "",
                Area = await context.SkillAreas.FirstAsync(x => x.Id == request.SkillAreaId, cancellationToken)
            };

            context.Skills.Add(skill);

            await context.SaveChangesAsync(cancellationToken);

            skill = await context
               .Skills
               .Include(x => x.Area)
               .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.Id == skill.Id, cancellationToken);

            return skill.ToDto();
        }
    }
}