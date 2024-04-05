using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.CompetenceAreas.Queries;

public record GetCompetenceAreaQuery(string Id) : IRequest<CompetenceAreaDto?>
{
    class GetCompetenceAreaQueryHandler : IRequestHandler<GetCompetenceAreaQuery, CompetenceAreaDto?>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetCompetenceAreaQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<CompetenceAreaDto?> Handle(GetCompetenceAreaQuery request, CancellationToken cancellationToken)
        {
            var competenceArea = await _context
               .CompetenceAreas
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (competenceArea is null)
            {
                return null;
            }

            return competenceArea.ToDto();
        }
    }
}