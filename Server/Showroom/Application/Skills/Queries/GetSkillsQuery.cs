
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Application.Common.Models;
using YourCompany.Showroom.Domain.Entities;

namespace YourCompany.Showroom.Application.Skills.Queries;

public class GetSkillsQuery : IRequest<Results<SkillDto>>
{
    public GetSkillsQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
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

    class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, Results<SkillDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSkillsQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Skill> result = _context
                    .Skills
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
                .Include(x => x.Area)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<SkillDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
