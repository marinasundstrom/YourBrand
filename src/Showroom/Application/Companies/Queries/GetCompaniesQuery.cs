
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Companies;

public record GetCompaniesQuery(int Page = 0, int PageSize = 10, int? IndustryId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<CompanyDto>>
{
    class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, Results<CompanyDto>>
    {
        private readonly IShowroomContext _context;
        private readonly IUserContext userContext;
        private readonly IUrlHelper _urlHelper;

        public GetCompaniesQueryHandler(
            IShowroomContext context,
            IUserContext userContext,
            IUrlHelper urlHelper)
        {
            _context = context;
            this.userContext = userContext;
            _urlHelper = urlHelper;
        }

        public async Task<Results<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Company> result = _context
                    .Companies
                    .AsNoTracking()
                    .AsQueryable();

            if (request.IndustryId is not null)
            {
                result = result.Where(p =>
                    p.Industry.Id == request.IndustryId);
            }

            if (request.SearchString is not null)
            {
                result = result.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }
            else
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Include(x => x.Industry)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items2 = items.Select(cp => cp.ToDto()).ToList();

            return new Results<CompanyDto>(items2, totalCount);
        }
    }
}