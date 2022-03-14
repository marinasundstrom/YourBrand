
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Domain.Entities;

namespace Skynet.Showroom.Application.ConsultantProfiles.Skills.Queries;

public class GetSkillsQuery : IRequest<Results<ConsultantProfileSkillDto>>
{
    public GetSkillsQuery(string consultantProfileId, int page = 0, int? pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        ConsultantProfileId = consultantProfileId;
        Page = page;
        PageSize = pageSize;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string? SearchString { get; }
    public string? SortBy { get; }
    public Common.Models.SortDirection? SortDirection { get; }
    public string ConsultantProfileId { get; }
    public int Page { get; }
    public int? PageSize { get; }

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
                    .Include(x => x.Skill)
                    .Where(x => x.ConsultantProfileId == request.ConsultantProfileId)
                    .OrderBy(o => o.Created)
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
                items = result.AsQueryable();
            }
            else 
            {
                items = result
                    .Skip((request.Page) * request.PageSize.GetValueOrDefault())
                    .Take(request.PageSize.GetValueOrDefault());
            }

            return new Results<ConsultantProfileSkillDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
