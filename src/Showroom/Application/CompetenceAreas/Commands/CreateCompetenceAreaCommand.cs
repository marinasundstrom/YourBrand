using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Commands;

public record CreateCompetenceAreaCommand(string Name) : IRequest<CompetenceAreaDto>
{
    public class CreateCompetenceAreaCommandHandler(IShowroomContext context) : IRequestHandler<CreateCompetenceAreaCommand, CompetenceAreaDto>
    {
        public async Task<CompetenceAreaDto> Handle(CreateCompetenceAreaCommand request, CancellationToken cancellationToken)
        {
            var competenceArea = await context.CompetenceAreas.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (competenceArea is not null) throw new Exception();

            competenceArea = new Domain.Entities.CompetenceArea
            {
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