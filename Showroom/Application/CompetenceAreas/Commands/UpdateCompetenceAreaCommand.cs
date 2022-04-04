using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Commands;

public record UpdateCompetenceAreaCommand(string Id, string Name) : IRequest
{
    public class UpdateCompetenceAreaCommandHandler : IRequestHandler<UpdateCompetenceAreaCommand>
    {
        private readonly IShowroomContext context;

        public UpdateCompetenceAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateCompetenceAreaCommand request, CancellationToken cancellationToken)
        {
            var competenceArea = await context.CompetenceAreas.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (competenceArea is null) throw new Exception();

            competenceArea.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
