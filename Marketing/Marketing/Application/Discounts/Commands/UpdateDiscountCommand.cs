using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Commands;

public record UpdateDiscountCommand(string Id, 
                string ProductId, 
                string ProductName, 
                string ProductDescription, 
                decimal OrdinaryPrice, 
                double Percent) : IRequest
{
    public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand>
    {
        private readonly IMarketingContext context;

        public UpdateDiscountCommandHandler(IMarketingContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await context.Discounts.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (discount is null) throw new Exception();

            //discount.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
