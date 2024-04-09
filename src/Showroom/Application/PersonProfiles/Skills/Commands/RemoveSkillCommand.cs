using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Commands;

public record RemoveSkillCommand(string Id) : IRequest
{
    public class RemoveSkillCommandHandler(IShowroomContext context) : IRequestHandler<RemoveSkillCommand>
    {
        public async Task Handle(RemoveSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.PersonProfileSkills
                .FirstOrDefaultAsync(i => i.Skill.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            context.PersonProfileSkills.Remove(skill);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}