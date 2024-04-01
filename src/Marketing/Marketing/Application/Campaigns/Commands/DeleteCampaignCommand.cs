using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Commands;

public record DeleteCampaignCommand(string Id) : IRequest
{
    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand>
    {
        private readonly IMarketingContext context;

        public DeleteCampaignCommandHandler(IMarketingContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaigns = await context.Campaigns
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (campaigns is null) throw new Exception();

            context.Campaigns.Remove(campaigns);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}