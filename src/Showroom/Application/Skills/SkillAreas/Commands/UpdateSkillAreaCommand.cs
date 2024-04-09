using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Commands;

public record UpdateSkillAreaCommand(string Id, string Name, int IndustryId) : IRequest
{
    public class UpdateSkillAreaCommandHandler(IShowroomContext context) : IRequestHandler<UpdateSkillAreaCommand>
    {
        public async Task Handle(UpdateSkillAreaCommand request, CancellationToken cancellationToken)
        {
            var skillArea = await context.SkillAreas.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skillArea is null) throw new Exception();

            skillArea.Name = request.Name;
            skillArea.Industry = await context.Industries.FirstAsync(x => x.Id == request.IndustryId, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}