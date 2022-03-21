using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Application.Common.Models;
using YourCompany.Showroom.Application.CompetenceAreas;
using YourCompany.Showroom.Domain.Entities;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.CompetenceAreas.Queries;

public class GetCompetenceAreasQuery : IRequest<Results<CompetenceAreaDto>>
{
    public GetCompetenceAreasQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string? SearchString { get; }
    public string? SortBy { get; }
    public Common.Models.SortDirection? SortDirection { get; }
    public int Page { get; }
    public int PageSize { get; }

    class GetCompetenceAreasQueryHandler : IRequestHandler<GetCompetenceAreasQuery, Results<CompetenceAreaDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetCompetenceAreasQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<CompetenceAreaDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
