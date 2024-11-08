using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Minutes.Command;

public record AddMinutesItem(string OrganizationId, int Id, int? AgendaId, string? AgendaItemId, int Type, string Title, string Description, int? MotionId, int? Order) : IRequest<Result<MinutesItemDto>>
{
    public class Validator : AbstractValidator<AddMinutesItem>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<SecretaryHub, ISecretaryHubClient> hubContext) : IRequestHandler<AddMinutesItem, Result<MinutesItemDto>>
    {
        public async Task<Result<MinutesItemDto>> Handle(AddMinutesItem request, CancellationToken cancellationToken)
        {
            var minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            var type = AgendaItemType.AllTypes.FirstOrDefault(x => x.Id == request.Type);

            if (type is null)
            {
                throw new Exception("Invalid type");
            }

            var minuteItem = minute.AddItem(type, request.AgendaId, request.AgendaItemId, request.Title, request.Description);

            if (request.Order is not null)
            {
                minute.ReorderItem(minuteItem, request.Order.GetValueOrDefault());
            }

            minuteItem.MotionId = request.MotionId;

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