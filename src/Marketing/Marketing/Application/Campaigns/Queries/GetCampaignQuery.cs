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
        private readonly ICurrentUserService currentUserService;

        public GetCampaignQueryHandler(
            IMarketingContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
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