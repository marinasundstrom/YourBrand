using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Queries;

public record GetDiscountQuery(string Id) : IRequest<DiscountDto?>
{
    class GetDiscountQueryHandler(
        IMarketingContext context,
        IUserContext userContext) : IRequestHandler<GetDiscountQuery, DiscountDto?>
    {
        public async Task<DiscountDto?> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var discount = await context
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