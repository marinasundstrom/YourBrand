using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Commands;

public record CreateDiscountCommand(
                string ProductId, 
                string ProductName, 
                string ProductDescription, 
                decimal OrdinaryPrice, 
                double Percent) : IRequest<DiscountDto>
{
    public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, DiscountDto>
    {
        private readonly IMarketingContext context;

        public CreateDiscountCommandHandler(IMarketingContext context)
        {
            this.context = context;
        }

        public async Task<DiscountDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = new Domain.Entities.Discount(
                request.ProductId, 
                request.ProductName, 
                request.ProductDescription, 
                request.OrdinaryPrice, 
                request.Percent);

            context.Discounts.Add(discount);
            
            await context.SaveChangesAsync(cancellationToken);

            return discount.ToDto();
        }
    }
}
