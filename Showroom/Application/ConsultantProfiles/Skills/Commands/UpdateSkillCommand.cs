using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record UpdateSkillCommand(string Id, SkillLevel Level, string? Comment) : IRequest<ConsultantProfileSkillDto>
{
    public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, ConsultantProfileSkillDto>
    {
        private readonly IShowroomContext context;

        public UpdateSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<ConsultantProfileSkillDto> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.ConsultantProfileSkills
               .Include(x => x.Skill)
               .ThenInclude(x => x.Area)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            skill.Level = request.Level;
            skill.Comment = request.Comment;

            await context.SaveChangesAsync(cancellationToken);

            return skill.ToDto();
        }
    }
}
