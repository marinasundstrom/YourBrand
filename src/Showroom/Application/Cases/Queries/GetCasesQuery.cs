using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Cases.Queries;

public record GetCasesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<CaseDto>>
{
    sealed class GetCasesQueryHandler(
        IShowroomContext context,
        IUrlHelper urlHelper) : IRequestHandler<GetCasesQuery, Results<CaseDto>>
    {
        public async Task<Results<CaseDto>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Case> result = context
                    .Cases
                     .OrderBy(o => o.Created)
                     .AsNoTracking()
                     .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(ca => ca.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            var items = await result
                .Include(c => c.CaseProfiles)
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<CaseDto>(items.Select(cp => cp.ToDto(urlHelper)), totalCount);
        }
    }
}