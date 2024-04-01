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
        private readonly ICurrentUserService currentUserService;

        public GetCompetenceAreaQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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