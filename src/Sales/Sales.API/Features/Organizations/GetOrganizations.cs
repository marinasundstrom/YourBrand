using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Models;
using YourBrand.Sales.Services;

namespace YourBrand.Sales.Features.OrderManagement.Organizations;

public record GetOrganizations(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<OrganizationDto>>
{
    public class Handler(IOrganizationRepository organizationRepository) : IRequestHandler<GetOrganizations, PagedResult<OrganizationDto>>
    {
        public async Task<PagedResult<OrganizationDto>> Handle(GetOrganizations request, CancellationToken cancellationToken)
        {
            var query = organizationRepository.GetAll();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SearchTerm is not null)
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var users = await query
                .OrderBy(i => i.Id)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<OrganizationDto>(users.Select(x => x.ToDto()), totalCount);
        }
    }
}