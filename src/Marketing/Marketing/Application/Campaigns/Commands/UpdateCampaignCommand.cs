﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Campaigns.Commands;

public record UpdateCampaignCommand(string Id, string Name) : IRequest
{
    public class UpdateCampaignCommandHandler(IMarketingContext context) : IRequestHandler<UpdateCampaignCommand>
    {
        public async Task Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaigns = await context.Campaigns.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (campaigns is null) throw new Exception();

            campaigns.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}