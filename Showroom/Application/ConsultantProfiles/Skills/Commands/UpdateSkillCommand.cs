using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Commands;

public record UpdateSkillCommand(string Id, SkillLevel Level, string? Comment) : IRequest
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
            var skill = await context.ConsultantProfileSkills.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skill is null) throw new Exception();

            skill.Level = request.Level;
            skill.Comment = request.Comment;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
