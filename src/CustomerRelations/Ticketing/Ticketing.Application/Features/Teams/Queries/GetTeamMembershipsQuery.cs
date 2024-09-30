using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Teams
.Queries;

public record GetTeamMembershipsQuery(string OrganizationId, string Id, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamMembershipDto>>
{
    sealed class GetTeamMembershipsQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext) : IRequestHandler<GetTeamMembershipsQuery, ItemsResult<TeamMembershipDto>>
    {
        public async Task<ItemsResult<TeamMembershipDto>> Handle(GetTeamMembershipsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<TeamMembership> result = context
                    .TeamMemberships
                    .InOrganization(request.OrganizationId)
                    .OrderBy(o => o.Created)
                    .Where(t => t.TeamId == request.Id)
                    .AsNoTracking()
                    .AsQueryable();

            /*
            if (request.SearchString is not null)
            {
                result = result.Where(o =>
                    o.User.FirstName.ToLower().Contains(request.SearchString.ToLower()) ||
                    o.User.LastName.ToLower().Contains(request.SearchString.ToLower()));
            } */

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection);
            }

            var items = await result
                .Include(x => x.User)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<TeamMembershipDto>(items.Select(cp => cp.ToDto2()), totalCount);
        }
    }
}