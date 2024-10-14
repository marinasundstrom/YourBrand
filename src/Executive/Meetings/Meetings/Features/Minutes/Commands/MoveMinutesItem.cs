using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Minutes.Command;

public record MoveMinutesItem(string OrganizationId, int Id, string ItemId, int Order) : IRequest<Result<MinutesItemDto>>
{
    public class Validator : AbstractValidator<MoveMinutesItem>
    {
        public Validator()
        {
            // RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<SecretaryHub, ISecretaryHubClient> hubContext) : IRequestHandler<MoveMinutesItem, Result<MinutesItemDto>>
    {
        public async Task<Result<MinutesItemDto>> Handle(MoveMinutesItem request, CancellationToken cancellationToken)
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

            minute.MoveItem(minuteItem, request.Order);

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

            return minuteItem.ToDto();
        }
    }
}