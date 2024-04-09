using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.Commands;

public record DeleteSkillCommand(string Id) : IRequest
{
    public class DeleteSkillCommandHandler(IShowroomContext context) : IRequestHandler<DeleteSkillCommand>
    {
        public async Task Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var skillArea = await context.Skills
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skillArea is null) throw new Exception();

            context.Skills.Remove(skillArea);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}