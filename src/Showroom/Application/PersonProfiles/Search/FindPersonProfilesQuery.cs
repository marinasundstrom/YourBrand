
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.PersonProfiles.Search.Queries;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Queries;

public record FindPersonProfilesQuery(PersonProfileQuery Query, int Page = 0, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<PersonProfileDto>>
{
    class FindPersonProfilesQueryHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<FindPersonProfilesQuery, Results<PersonProfileDto>>
    {
        public async Task<Results<PersonProfileDto>> Handle(FindPersonProfilesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PersonProfile> result = context
                    .PersonProfiles
                    .AsNoTracking()
                    .AsQueryable();

            /*
            if (request.AvailableFrom != null)
            {
                request.AvailableFrom = request.AvailableFrom?.Date;
                result = result.Where(e => e.AvailableFromDate == null || request.AvailableFrom >= e.AvailableFromDate);
            }
            */

            if (request.Query is not null)
            {
                var query = request.Query;

                if (query.IndustryId is not null)
                {
                    result = result.Where(p => p.IndustryId == query.IndustryId);
                }

                if (query.OrganizationId is not null)
                {
                    result = result.Where(p => p.OrganizationId == query.OrganizationId);
                }

                if (query.CompetenceAreaId is not null)
                {
                    result = result.Where(p => p.CompetenceAreaId == query.CompetenceAreaId);
                }

                if (query.Skills?.Any() ?? false)
                {
                    foreach (var skill in request.Query.Skills)
                    {
                        result = result.Where(p => p.PersonProfileSkills.Any(x => x.SkillId == skill.SkillId && x.Level >= skill.Level));
                    }
                }

                if (query.Experiences?.Any() ?? false)
                {
                    foreach (var industry in request.Query.Experiences)
                    {
                        result = result.Where(p =>
                            p.IndustryExperience
                            .Where(x => x.Industry.Id == industry.IndustryId)
                            .Where(x => x.Years >= industry.Years).Any());
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

            var items2 = items.Select(cp => cp.ToDto(urlHelper)).ToList();

            return new Results<PersonProfileDto>(items2, totalCount);
        }
    }
}