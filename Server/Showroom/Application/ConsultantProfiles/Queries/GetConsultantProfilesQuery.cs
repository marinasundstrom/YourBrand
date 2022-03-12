
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Domain.Entities;

namespace Skynet.Showroom.Application.ConsultantProfiles.Queries;

public class GetConsultantProfilesQuery : IRequest<Results<ConsultantProfileDto>>
{
    public GetConsultantProfilesQuery(int page = 0, int pageSize = 10, string? organizationId = null, string? competenceAreaId = null, DateTime? availableFrom = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        OrganizationId = organizationId;
        CompetenceAreaId = competenceAreaId;
        AvailableFrom = availableFrom;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string? OrganizationId { get; }
    public string? CompetenceAreaId { get; }
    public DateTime? AvailableFrom { get; private set; }
    public string? SearchString { get; }
    public string? SortBy { get; }
    public Common.Models.SortDirection? SortDirection { get; }
    public int Page { get; }
    public int PageSize { get; }

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
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Include(x => x.Organization)
                    .Include(c => c.CompetenceArea)
                    //.Include(c => c.Manager)
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

            var items = await result
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto(_urlHelper)).ToList();

            return new Results<ConsultantProfileDto>(items2, totalCount);
        }
    }
}
