using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record UpdateSkillExperiencesCommand(string Id, string ConsultantProfileSkillId, IEnumerable<UpdateSkillExperienceDto> Experiences) : IRequest
{
    public class Handler : IRequestHandler<UpdateSkillExperiencesCommand>
    {
        private readonly IShowroomContext _context;

        public Handler(IShowroomContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateSkillExperiencesCommand request, CancellationToken cancellationToken)
        {
            foreach (var experience in request.Experiences)
            {
                var exp = await _context.ConsultantProfileExperiences
                    .Include(x => x.Skills)
                    .Where(x => x.ConsultantProfile.Id == request.Id)
                    .FirstAsync(x => x.Id == experience.ConsultantProfileExperienceId);

                var skill = exp.Skills.FirstOrDefault(x => x.ConsultantProfileSkillId == request.ConsultantProfileSkillId);

                if (experience.Checked)
                {
                    if (skill is null) 
                    {
                        exp.Skills.Add(new Domain.Entities.ConsultantProfileExperienceSkill()
                        {
                            Id = Guid.NewGuid().ToString(),
                            ConsultantProfileExperience = exp,
                            ConsultantProfileSkill = await _context.ConsultantProfileSkills.FirstAsync(x => x.Id == request.ConsultantProfileSkillId, cancellationToken),
                        });
                    }
                }
                else
                {
                    if (skill is not null)
                    {
                        exp.Skills.Remove(skill);
                        _context.ConsultantProfileExperienceSkills.Remove(skill);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
