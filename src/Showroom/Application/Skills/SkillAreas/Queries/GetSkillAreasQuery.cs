
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Queries;

public record GetSkillAreasQuery(int Page = 0, int PageSize = 10, int? IndustryId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<SkillAreaDto>>
{
    class GetSkillAreasQueryHandler : IRequestHandler<GetSkillAreasQuery, Results<SkillAreaDto>>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;

        public GetSkillAreasQueryHandler(
            IShowroomContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<Results<SkillAreaDto>> Handle(GetSkillAreasQuery request, CancellationToken cancellationToken)
        {
            IQueryable<SkillArea> result = _context
                    .SkillAreas
                     .OrderBy(o => o.Created)
                     //.AsNoTracking()
                     .AsQueryable();

            if (request.IndustryId is not null)
            {
                result = result.Where(p =>
                    p.Industry.Id == request.IndustryId);
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
                .Include(x => x.Industry)
                .Include(x => x.Skills)
                //.ThenInclude(x => x.Area)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<SkillAreaDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}