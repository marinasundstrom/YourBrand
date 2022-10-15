using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Queries;

public record GetDiscountQuery(string Id) : IRequest<DiscountDto?>
{
    class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, DiscountDto?>
    {
        private readonly IMarketingContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetDiscountQueryHandler(
            IMarketingContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<DiscountDto?> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var discount = await _context
               .Discounts
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (discount is null)
            {
                return null;
            }

            return discount.ToDto();
        }
    }
}
