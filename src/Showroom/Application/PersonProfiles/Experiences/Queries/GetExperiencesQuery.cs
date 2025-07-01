
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Queries;

public enum ExperiencesFilter { All = 1, Employments, Assignments }

public enum ExperiencesDisplayMode { Flat = 1, Hierarchical }

public record GetExperiencesQuery(ExperiencesFilter Filter, ExperiencesDisplayMode DisplayMode, int Page = 0, int? PageSize = 10, string? PersonProfileId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<ExperienceDto>>
{
    sealed class GetExperiencesQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetExperiencesQuery, Results<ExperienceDto>>
    {
        public async Task<Results<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Employment> result = context
                    .Employments
                    .Include(x => x.Assignments) //?
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.EndDate)
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrEmpty(request.PersonProfileId))
            {
                result = result.Where(e => e.PersonProfile.Id == request.PersonProfileId);
            }

            if (request.SearchString is not null)
            {
                /*
                result = result.Where(p =>
                    p.Company.Name.ToLower().Contains(request.SearchString.ToLower())
                    || p.Location!.ToLower().Contains(request.SearchString.ToLower())
                    || p.Title.ToLower().Contains(request.SearchString.ToLower()));
                */
            }

            var totalCount = await result
                .CountAsync(cancellationToken);

            totalCount += await result
                .SelectMany(x => x.Assignments)
                .CountAsync(cancellationToken);

            result = result
                .Include(x => x.Employer)
                .ThenInclude(x => x.Industry)
                .Include(x => x.Assignments)
                .ThenInclude(x => x.Company)
                .ThenInclude(x => x.Industry)

                .Include(x => x.Skills)
                .ThenInclude(x => x.PersonProfileSkill)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)

                .Include(x => x.Roles)
                .ThenInclude(x => x.Skills)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)

                .Include(x => x.Assignments)
                .Include(x => x.Employer)
                .Include(x => x.Assignments)
                .ThenInclude(x => x.Roles)

                .Include(x => x.Assignments)
                .ThenInclude(x => x.Skills)
                .ThenInclude(x => x.PersonProfileSkill)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            IQueryable<Employment> items = null!;

            if (request.PageSize is null)
            {
                items = result.AsQueryable();
            }
            else
            {
                items = result
                    .Skip((request.Page) * request.PageSize.GetValueOrDefault())
                    .Take(request.PageSize.GetValueOrDefault());
            }

            List<ExperienceDto> experience = new List<ExperienceDto>();

            var flatDisplayMode = request.DisplayMode == ExperiencesDisplayMode.Flat;
            var hierarchicalDisplayMode = !flatDisplayMode;

            foreach (var item in items)
            {
                experience.Add(item.ToDto(hierarchicalDisplayMode));

                if (flatDisplayMode)
                {
                    foreach (var assignment in item.Assignments)
                    {
                        experience.Add(assignment.ToDto());
                    }
                }
            }

            return new Results<ExperienceDto>(
                experience
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.EndDate),
                totalCount);
        }
    }
}
