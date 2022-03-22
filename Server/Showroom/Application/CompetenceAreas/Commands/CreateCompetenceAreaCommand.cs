using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Commands;

public record CreateCompetenceAreaCommand(string Name) : IRequest
{
    public class CreateCompetenceAreaCommandHandler : IRequestHandler<CreateCompetenceAreaCommand>
    {
        private readonly IShowroomContext context;

        public CreateCompetenceAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateCompetenceAreaCommand request, CancellationToken cancellationToken)
        {
            var competenceArea = await context.CompetenceAreas.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (competenceArea is not null) throw new Exception();

            competenceArea = new Domain.Entities.CompetenceArea
            {
                Name = request.Name
            };

            context.CompetenceAreas.Add(competenceArea);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
