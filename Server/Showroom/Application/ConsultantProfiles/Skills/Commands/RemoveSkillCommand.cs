using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record RemoveSkillCommand(string Id) : IRequest
{
    public class RemoveSkillCommandHandler : IRequestHandler<RemoveSkillCommand>
    {
        private readonly IShowroomContext context;

        public RemoveSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.ConsultantProfileSkills
                .FirstOrDefaultAsync(i => i.SkillId == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            context.ConsultantProfileSkills.Remove(skill);
           
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}