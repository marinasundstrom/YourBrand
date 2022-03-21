using MediatR;

using Microsoft.EntityFrameworkCore;
using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.Application.Skills.Commands;

public record DeleteSkillCommand(string Id) : IRequest
{
    public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand>
    {
        private readonly IShowroomContext context;

        public DeleteSkillCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var skillArea = await context.Skills
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (skillArea is null) throw new Exception();

            context.Skills.Remove(skillArea);
           
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}