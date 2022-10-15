using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Marketing.Domain.Entities;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Queries;

public record GetCampaignsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<CampaignDto>>
{
    class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, ItemsResult<CampaignDto>>
    {
        private readonly IMarketingContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetCampaignsQueryHandler(
            IMarketingContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ItemsResult<CampaignDto>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Campaign> result = _context
                    .Campaigns
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Marketing.Application.SortDirection.Descending : Marketing.Application.SortDirection.Ascending);
            }
            else 
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<CampaignDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
