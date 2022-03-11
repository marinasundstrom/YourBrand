
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Domain.Entities;

namespace Skynet.Showroom.Application.ConsultantProfiles.Queries;

public class GetExperiencesQuery : IRequest<Results<ExperienceDto>>
{
    public GetExperiencesQuery(int page = 0, int pageSize = 10, string? consultantProfileId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        ConsultantProfileId = consultantProfileId;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public string? ConsultantProfileId { get; }
    public string? SearchString { get; }
    public string? SortBy { get; }
    public Common.Models.SortDirection? SortDirection { get; }
    public int Page { get; }
    public int PageSize { get; }

    class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, Results<ExperienceDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetExperiencesQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ConsultantProfileExperience> result = _context
                    .ConsultantProfileExperiences
                    .OrderByDescending(x => x.StartDate)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(request.ConsultantProfileId))
            {
                result = result.Where(e => e.ConsultantProfile.Id == request.ConsultantProfileId);
            }

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.CompanyName.ToLower().Contains(request.SearchString.ToLower())
                    || p.Location!.ToLower().Contains(request.SearchString.ToLower())
                    || p.Title.ToLower().Contains(request.SearchString.ToLower()));
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

            return new Results<ExperienceDto>(
                items.Select(e => e.ToDto()), 
                totalCount);
        }
    }
}
