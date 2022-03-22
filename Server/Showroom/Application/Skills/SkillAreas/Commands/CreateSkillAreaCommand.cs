using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Commands;

public record CreateSkillAreaCommand(string Name) : IRequest
{
    public class CreateSkillAreaCommandHandler : IRequestHandler<CreateSkillAreaCommand>
    {
        private readonly IShowroomContext context;

        public CreateSkillAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateSkillAreaCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.SkillAreas.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (skill is not null) throw new Exception();

            skill = new Domain.Entities.SkillArea
            {
                Name = request.Name
            };

            context.SkillAreas.Add(skill);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
