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
        private readonly IUserContext userContext;

        public GetDiscountQueryHandler(
            IMarketingContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
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