using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Commands;

public record UpdateDiscountCommand(string Id,
                string ItemId,
                string ItemName,
                string ItemDescription,
                double Percentage,
                decimal Amount) : IRequest
{
    public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand>
    {
        private readonly IMarketingContext context;

        public UpdateDiscountCommandHandler(IMarketingContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await context.Discounts.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (discount is null) throw new Exception();

            //discount.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
