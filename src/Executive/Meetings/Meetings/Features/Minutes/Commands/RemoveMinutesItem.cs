using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Minutes.Command;

public record RemoveMinutesItem(string OrganizationId, int Id, string ItemId) : IRequest<Result<MinutesDto>>
{
    public class Validator : AbstractValidator<RemoveMinutesItem>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<SecretaryHub, ISecretaryHubClient> hubContext) : IRequestHandler<RemoveMinutesItem, Result<MinutesDto>>
    {
        public async Task<Result<MinutesDto>> Handle(RemoveMinutesItem request, CancellationToken cancellationToken)
        {
            var minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            var minuteItem = minute.Items.FirstOrDefault(x => x.Id == request.ItemId);

            if (minuteItem is null)
            {
                return Errors.Minutes.MinutesItemNotFound;
            }

            minute.RemoveItem(minuteItem);

            context.Minutes.Update(minute);

            await context.SaveChangesAsync(cancellationToken);

            minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == minute.Id!, cancellationToken);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            await hubContext.Clients
                .Group($"meeting-{minute.MeetingId}")
                .OnMinutesUpdated();

            return minute.ToDto();
        }
    }
}