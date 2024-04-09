
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

public record GetSkillsQuery(string PersonProfileId, int Page = 0, int? PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<PersonProfileSkillDto>>
{
    sealed class GetSkillsQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetSkillsQuery, Results<PersonProfileSkillDto>>
    {
        public async Task<Results<PersonProfileSkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PersonProfileSkill> result = context
                    .PersonProfileSkills
                    .Where(x => x.PersonProfileId == request.PersonProfileId)
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
            else
            {
                result = result.OrderBy(x => x.Skill.Name);
            }

            IQueryable<PersonProfileSkill> items = null!;

            if (request.PageSize is null)
            {
                items = result
                    .Include(x => x.Skill)
                    .ThenInclude(x => x.Area)
                    .ThenInclude(x => x.Industry)
                    .AsQueryable();
            }
            else
            {
                items = result
                    .Include(x => x.Skill)
                    .ThenInclude(x => x.Area)
                    .ThenInclude(x => x.Industry)
                    .Skip((request.Page) * request.PageSize.GetValueOrDefault())
                    .Take(request.PageSize.GetValueOrDefault());
            }

            return new Results<PersonProfileSkillDto>(
                items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}