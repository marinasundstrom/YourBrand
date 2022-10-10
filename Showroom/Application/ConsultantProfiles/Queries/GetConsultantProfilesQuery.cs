
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Queries;

public record GetConsultantProfilesQuery(int Page = 0, int PageSize = 10, string? OrganizationId = null, string? CompetenceAreaId = null, DateTime? AvailableFrom = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<ConsultantProfileDto>>
{
    class GetConsultantProfilesQueryHandler : IRequestHandler<GetConsultantProfilesQuery, Results<ConsultantProfileDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IUrlHelper _urlHelper;

        public GetConsultantProfilesQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<Results<ConsultantProfileDto>> Handle(GetConsultantProfilesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ConsultantProfile> result = _context
                    .ConsultantProfiles
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrEmpty(request.OrganizationId))
            {
                var organization = await _context.Organizations.FindAsync(request.OrganizationId);
                if (organization == null)
                {
                    throw new Exception("Org not found");
                }

                result = result.Where(e => e.OrganizationId == request.OrganizationId);
            }

            if (request.CompetenceAreaId != null)
            {
                result = result.Where(e => e.CompetenceAreaId == request.CompetenceAreaId);
            }

            /*
            if (request.AvailableFrom != null)
            {
                request.AvailableFrom = request.AvailableFrom?.Date;
                result = result.Where(e => e.AvailableFromDate == null || request.AvailableFrom >= e.AvailableFromDate);
            }
            */

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || p.LastName.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }
            else 
            {
                result = result
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName);
            }

            var items = await result
                .Include(x => x.Industry)
                .Include(x => x.Organization)
                .Include(c => c.CompetenceArea)
                //.Include(c => c.Manager)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto(_urlHelper)).ToList();

            return new Results<ConsultantProfileDto>(items2, totalCount);
        }
    }
}
