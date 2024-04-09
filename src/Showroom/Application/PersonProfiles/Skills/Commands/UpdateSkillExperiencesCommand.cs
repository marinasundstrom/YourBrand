using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Commands;

public record UpdateSkillExperiencesCommand(string Id, string PersonProfileSkillId, IEnumerable<UpdateSkillExperienceDto> Experiences) : IRequest
{
    public class Handler(IShowroomContext context) : IRequestHandler<UpdateSkillExperiencesCommand>
    {
        public async Task Handle(UpdateSkillExperiencesCommand request, CancellationToken cancellationToken)
        {
            foreach (var experience in request.Experiences)
            {
                var exp = await context.PersonProfileExperiences
                    .Include(x => x.Skills)
                    .Where(x => x.PersonProfile.Id == request.Id)
                    .FirstAsync(x => x.Id == experience.PersonProfileExperienceId);

                var skill = exp.Skills.FirstOrDefault(x => x.PersonProfileSkillId == request.PersonProfileSkillId);

                if (experience.Checked)
                {
                    if (skill is null)
                    {
                        exp.Skills.Add(new Domain.Entities.PersonProfileExperienceSkill()
                        {
                            Id = Guid.NewGuid().ToString(),
                            PersonProfileExperience = exp,
                            PersonProfileSkill = await context.PersonProfileSkills.FirstAsync(x => x.Id == request.PersonProfileSkillId, cancellationToken),
                        });
                    }
                }
                else
                {
                    if (skill is not null)
                    {
                        exp.Skills.Remove(skill);
                        context.PersonProfileExperienceSkills.Remove(skill);
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}