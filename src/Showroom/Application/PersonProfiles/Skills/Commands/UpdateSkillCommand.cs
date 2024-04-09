using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Commands;

public record UpdateSkillCommand(string Id, SkillLevel Level, string? Comment) : IRequest<PersonProfileSkillDto>
{
    public class UpdateSkillCommandHandler(IShowroomContext context) : IRequestHandler<UpdateSkillCommand, PersonProfileSkillDto>
    {
        public async Task<PersonProfileSkillDto> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.PersonProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
               .ThenInclude(x => x.Industry)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            skill.Level = request.Level;
            skill.Comment = request.Comment;

            await context.SaveChangesAsync(cancellationToken);

            return skill.ToDto();
        }
    }
}