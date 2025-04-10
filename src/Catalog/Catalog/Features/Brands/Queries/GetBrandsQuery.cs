﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Brands.Queries;

public sealed record GetBrandsQuery(string OrganizationId, string? ProductCategoryIdOrPath, int Page = 1, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<BrandDto>>
{
    sealed class GetBrandsQueryHandler(
        CatalogContext context,
        IUserContext userContext) : IRequestHandler<GetBrandsQuery, PagedResult<BrandDto>>
    {
        public async Task<PagedResult<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Brand> result = context
                    .Brands
                .InOrganization(request.OrganizationId)
                     //.OrderBy(o => o.Created)
                     .AsNoTracking()
                     .AsQueryable();

            if (!string.IsNullOrEmpty(request.ProductCategoryIdOrPath))
            {
                bool isProductCategoryId = long.TryParse(request.ProductCategoryIdOrPath, out var categoryId);

                result = isProductCategoryId
                            ? result.Where(brand => context.Products.Any(p => p.BrandId == brand.Id && p.CategoryId == categoryId))
                            : result.Where(brand => context.Products.Any(p => p.BrandId == brand.Id && p.Category!.Path.StartsWith(request.ProductCategoryIdOrPath)));
            }

            if (request.SearchString is not null)
            {
                result = result.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .InOrganization(request.OrganizationId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<BrandDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}