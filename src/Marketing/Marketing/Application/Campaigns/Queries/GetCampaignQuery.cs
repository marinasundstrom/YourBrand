using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Queries;

public record GetCampaignQuery(string Id) : IRequest<CampaignDto?>
{
    sealed class GetCampaignQueryHandler(
        IMarketingContext context,
        IUserContext userContext) : IRequestHandler<GetCampaignQuery, CampaignDto?>
    {
        public async Task<CampaignDto?> Handle(GetCampaignQuery request, CancellationToken cancellationToken)
        {
            var campaigns = await context
               .Campaigns
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (campaigns is null)
            {
                return null;
            }

            return campaigns.ToDto();
        }
    }
}