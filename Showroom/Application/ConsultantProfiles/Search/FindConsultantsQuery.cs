
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Queries;

public record FindConsultantsQuery(ConsultantQuery Query, int Page = 0, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<ConsultantProfileDto>>
{
    class FindConsultantsQueryHandler : IRequestHandler<FindConsultantsQuery, Results<ConsultantProfileDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public FindConsultantsQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<Results<ConsultantProfileDto>> Handle(FindConsultantsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ConsultantProfile> result = _context
                    .ConsultantProfiles
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Include(x => x.Organization)
                    .Include(c => c.CompetenceArea)
                    //.Include(c => c.Manager)
                    .AsNoTracking()
                    .AsQueryable();

            /*
            if (request.AvailableFrom != null)
            {
                request.AvailableFrom = request.AvailableFrom?.Date;
                result = result.Where(e => e.AvailableFromDate == null || request.AvailableFrom >= e.AvailableFromDate);
            }
            */

            if(request.Query is not null) 
            {
                var query = request.Query;

                if (query.OrganizationId is not null)
                {
                    result = result.Where(p => p.OrganizationId == query.OrganizationId);
                }

                if (query.CompetenceAreaId is not null)
                {
                    result = result.Where(p => p.CompetenceAreaId == query.CompetenceAreaId);
                }

                if(query.Skills?.Any() ?? false) 
                {
                    foreach(var skill in request.Query.Skills) 
                    {
                        result = result.Where(p => p.ConsultantProfileSkills.Any(x => x.SkillId == skill.SkillId && x.Level >= skill.Level));
                    }
                }

                if (query.SearchString is not null)
                {
                    result = result.Where(p =>
                        p.FirstName.ToLower().Contains(request.Query.SearchString.ToLower())
                        || p.LastName.ToLower().Contains(request.Query.SearchString.ToLower())
                        || p.Headline.ToLower().Contains(request.Query.SearchString.ToLower())
                        || p.Presentation.ToLower().Contains(request.Query.SearchString.ToLower()));
                }
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            var items = await result
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto(_urlHelper)).ToList();

            return new Results<ConsultantProfileDto>(items2, totalCount);
        }
    }
}
