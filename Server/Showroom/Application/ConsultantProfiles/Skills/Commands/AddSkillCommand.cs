using MediatR;

using Microsoft.EntityFrameworkCore;
using Skynet.Showroom.Application.Common.Interfaces;

namespace Skynet.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record AddSkillCommand(string ConsultantProfileId, string SkillId) : IRequest
{
    public class AddSkillCommandHandler : IRequestHandler<AddSkillCommand>
    {
        private readonly IShowroomContext context;

        public AddSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(AddSkillCommand request, CancellationToken cancellationToken)
        {
            var consultantProfile = await context.ConsultantProfiles
                .FirstOrDefaultAsync(i => i.Id == request.ConsultantProfileId, cancellationToken);

            if (consultantProfile is null) throw new Exception();

            var skill = await context.Skills
                .FirstOrDefaultAsync(i => i.Id == request.SkillId, cancellationToken);

            if (skill is null) throw new Exception();

            var consultantProfileSkill = await context.ConsultantProfileSkills
                .Where(x => x.ConsultantProfileId == request.ConsultantProfileId)
                .Include(x => x.Skill)
                .FirstOrDefaultAsync(i => i.Skill.Id == request.SkillId, cancellationToken);

            if (consultantProfileSkill is not null) throw new Exception();

            consultantProfileSkill = new Domain.Entities.ConsultantProfileSkill
            {
                Id = Guid.NewGuid().ToString(),
                ConsultantProfileId = consultantProfile.Id,
                Skill = skill,
            };

            context.ConsultantProfileSkills.Add(consultantProfileSkill);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
