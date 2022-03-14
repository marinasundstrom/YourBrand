
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Domain.Entities;

namespace Skynet.Showroom.Application.Skills.SkillAreas.Queries;

public class GetSkillAreasQuery : IRequest<Results<SkillAreaDto>>
{
    public GetSkillAreasQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
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

    class GetSkillAreasQueryHandler : IRequestHandler<GetSkillAreasQuery, Results<SkillAreaDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSkillAreasQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<SkillAreaDto>> Handle(GetSkillAreasQuery request, CancellationToken cancellationToken)
        {
            IQueryable<SkillArea> result = _context
                    .SkillAreas
                     .OrderBy(o => o.Created)
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

            return new Results<SkillAreaDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
