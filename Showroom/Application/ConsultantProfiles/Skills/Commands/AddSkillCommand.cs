using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record AddSkillCommand(string ConsultantProfileId, string SkillId, SkillLevel Level, string? Comment) : IRequest<ConsultantProfileSkillDto>
{
    public class AddSkillCommandHandler : IRequestHandler<AddSkillCommand, ConsultantProfileSkillDto>
    {
        private readonly IShowroomContext context;

        public AddSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<ConsultantProfileSkillDto> Handle(AddSkillCommand request, CancellationToken cancellationToken)
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
                Level = request.Level,
                Comment = request.Comment
            };

            context.ConsultantProfileSkills.Add(consultantProfileSkill);

            await context.SaveChangesAsync(cancellationToken);

            consultantProfileSkill = await context
               .ConsultantProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.SkillId, cancellationToken);

            return consultantProfileSkill.ToDto();
        }
    }
}
