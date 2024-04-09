using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Commands;

public record DeleteSkillAreaCommand(string Id) : IRequest
{
    public class DeleteSkillAreaCommandHandler(IShowroomContext context) : IRequestHandler<DeleteSkillAreaCommand>
    {
        public async Task Handle(DeleteSkillAreaCommand request, CancellationToken cancellationToken)
        {
            var skillArea = await context.SkillAreas
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skillArea is null) throw new Exception();

            context.SkillAreas.Remove(skillArea);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}