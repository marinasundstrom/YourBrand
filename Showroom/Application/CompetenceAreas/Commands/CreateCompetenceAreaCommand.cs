using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Commands;

public record CreateCompetenceAreaCommand(string Name) : IRequest<CompetenceAreaDto>
{
    public class CreateCompetenceAreaCommandHandler : IRequestHandler<CreateCompetenceAreaCommand, CompetenceAreaDto>
    {
        private readonly IShowroomContext context;

        public CreateCompetenceAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<CompetenceAreaDto> Handle(CreateCompetenceAreaCommand request, CancellationToken cancellationToken)
        {
            var competenceArea = await context.CompetenceAreas.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (competenceArea is not null) throw new Exception();

            competenceArea = new Domain.Entities.CompetenceArea
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name
            };

            context.CompetenceAreas.Add(competenceArea);

            await context.SaveChangesAsync(cancellationToken);

            competenceArea = await context
               .CompetenceAreas
               .AsNoTracking()
               .FirstAsync(c => c.Id == competenceArea.Id);

            return competenceArea.ToDto();
        }
    }
}
