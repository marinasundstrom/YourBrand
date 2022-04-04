using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record UpdateSkillCommand(string Id, string Name) : IRequest
{
    public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand>
    {
        private readonly IShowroomContext context;

        public UpdateSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.ConsultantProfileSkills.FirstOrDefaultAsync(i => i.SkillId == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            //skill.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
