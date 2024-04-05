using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.CompetenceAreas.Queries;

public record GetCompetenceAreasQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<CompetenceAreaDto>>
{
    class GetCompetenceAreasQueryHandler : IRequestHandler<GetCompetenceAreasQuery, Results<CompetenceAreaDto>>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetCompetenceAreasQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<Results<CompetenceAreaDto>> Handle(GetCompetenceAreasQuery request, CancellationToken cancellationToken)
        {
            IQueryable<CompetenceArea> result = _context
                    .CompetenceAreas
                     .OrderBy(o => o.Created)
                     .AsNoTracking()
                     .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }
            else
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<CompetenceAreaDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}