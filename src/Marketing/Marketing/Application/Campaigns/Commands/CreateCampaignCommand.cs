using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Commands;

public record CreateCampaignCommand(string Name) : IRequest<CampaignDto>
{
    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, CampaignDto>
    {
        private readonly IMarketingContext context;

        public CreateCampaignCommandHandler(IMarketingContext context)
        {
            this.context = context;
        }

        public async Task<CampaignDto> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaigns = await context.Campaigns.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (campaigns is not null) throw new Exception();

            campaigns = new Domain.Entities.Campaign(request.Name);

            context.Campaigns.Add(campaigns);

            await context.SaveChangesAsync(cancellationToken);

            return campaigns.ToDto();
        }
    }
}