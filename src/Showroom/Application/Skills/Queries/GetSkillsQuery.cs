
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Skills.Queries;

public record GetSkillsQuery(int Page = 0, int PageSize = 10, string? SkillAreaId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<SkillDto>>
{
    class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, Results<SkillDto>>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetSkillsQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<Results<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Skill> result = _context
                    .Skills
                    .OrderBy(o => o.Name)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SkillAreaId is not null)
            {
                result = result.Where(ca => ca.Area.Id == request.SkillAreaId);
            }

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
                .Include(x => x.Area)
                .ThenInclude(x => x.Industry)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<SkillDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}