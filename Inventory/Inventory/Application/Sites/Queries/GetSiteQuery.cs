using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Queries;

public record GetSiteQuery(string Id) : IRequest<SiteDto?>
{
    class GetSiteQueryHandler : IRequestHandler<GetSiteQuery, SiteDto?>
    {
        private readonly IInventoryContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetSiteQueryHandler(
            IInventoryContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<SiteDto?> Handle(GetSiteQuery request, CancellationToken cancellationToken)
        {
            var site = await _context
               .Sites
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (site is null)
            {
                return null;
            }

            return site.ToDto();
        }
    }
}
