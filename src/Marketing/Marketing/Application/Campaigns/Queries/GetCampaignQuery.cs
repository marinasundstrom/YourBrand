using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Queries;

public record GetCampaignQuery(string Id) : IRequest<CampaignDto?>
{
    class GetCampaignQueryHandler : IRequestHandler<GetCampaignQuery, CampaignDto?>
    {
        private readonly IMarketingContext _context;
        private readonly IUserContext userContext;

        public GetCampaignQueryHandler(
            IMarketingContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<CampaignDto?> Handle(GetCampaignQuery request, CancellationToken cancellationToken)
        {
            var campaigns = await _context
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