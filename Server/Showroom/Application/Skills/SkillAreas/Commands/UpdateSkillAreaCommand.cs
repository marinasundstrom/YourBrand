using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Skills.SkillAreas.Commands;

public record UpdateSkillAreaCommand(string Id, string Name) : IRequest
{
    public class UpdateSkillAreaCommandHandler : IRequestHandler<UpdateSkillAreaCommand>
    {
        private readonly IShowroomContext context;

        public UpdateSkillAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateSkillAreaCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.SkillAreas.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            skill.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
