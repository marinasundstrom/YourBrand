
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

public record GetSkillsQuery(string ConsultantProfileId, int Page = 0, int? PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<ConsultantProfileSkillDto>>
{
    class GetSkillsQueryHandler : IRequestHandler<GetSkillsQuery, Results<ConsultantProfileSkillDto>>
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

        public async Task<Results<ConsultantProfileSkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ConsultantProfileSkill> result = _context
                    .ConsultantProfileSkills
                    .Where(x => x.ConsultantProfileId == request.ConsultantProfileId)
                    .OrderBy(x => x.Skill.Name)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(ca => ca.Skill.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            IQueryable<ConsultantProfileSkill> items = null!;

            if(request.PageSize is null) 
            {
                items = result
                    .Include(x => x.Skill)
                    .ThenInclude(x => x.Area)
                    .AsQueryable();
            }
            else 
            {
                items = result
                    .Include(x => x.Skill)
                    .ThenInclude(x => x.Area)
                    .Skip((request.Page) * request.PageSize.GetValueOrDefault())
                    .Take(request.PageSize.GetValueOrDefault());
            }

            return new Results<ConsultantProfileSkillDto>(
                items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
