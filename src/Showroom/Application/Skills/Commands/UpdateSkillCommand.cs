using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.Commands;

public record UpdateSkillCommand(string Id, string Name) : IRequest
{
    public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand>
    {
        private readonly IShowroomContext context;

        public UpdateSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.Skills.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            skill.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}