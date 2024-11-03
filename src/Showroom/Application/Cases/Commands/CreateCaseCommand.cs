using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Cases.Commands;

public record CasePricing(decimal? HourlyPrice, double? Hours);

public record CreateCaseCommand(string? Description, CasePricing? Pricing) : IRequest<CaseDto>
{
    public class CreateCaseCommandHandler(IShowroomContext context, IUrlHelper urlHelper) : IRequestHandler<CreateCaseCommand, CaseDto>
    {
        public async Task<CaseDto> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
        {
            var @case = new Domain.Entities.Case
            {
                Status = Domain.Enums.CaseStatus.Created,
                Description = request.Description
            };

            var pricing = request.Pricing;

            if (pricing is not null)
            {
                @case.Pricing = new Domain.Entities.CasePricing
                {
                    HourlyPrice = pricing.HourlyPrice,
                    Hours = pricing.Hours,
                    Total = pricing.HourlyPrice * (decimal?)pricing.Hours
                };
            }

            context.Cases.Add(@case);

            await context.SaveChangesAsync(cancellationToken);

            @case = await context.Cases
                .Include(c => c.CaseProfiles)
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .FirstOrDefaultAsync(x => x.Id == @case.Id);

            return @case.ToDto(urlHelper);
        }
    }
}