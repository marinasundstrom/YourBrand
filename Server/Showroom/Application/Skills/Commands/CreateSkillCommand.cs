using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Skills.Commands;

public record CreateSkillCommand(string Name) : IRequest
{
    public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand>
    {
        private readonly IShowroomContext context;

        public CreateSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await context.Skills.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (skill is not null) throw new Exception();

            skill = new Domain.Entities.Skill
            {
                Name = request.Name
            };

            context.Skills.Add(skill);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
