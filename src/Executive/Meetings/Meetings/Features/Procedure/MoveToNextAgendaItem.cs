using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record MoveToNextAgendaItem(string OrganizationId, int Id) : IRequest<Result<AgendaItemDto>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<MoveToNextAgendaItem, Result<AgendaItemDto>>
    {
        public async Task<Result<AgendaItemDto>> Handle(MoveToNextAgendaItem request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId!);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            var chairFunction = meeting.GetChairpersonFunction(attendee);

            if (chairFunction is null)
            {
                return Errors.Meetings.OnlyChairpersonCanMoveToNextAgendaItem;
            }

            var nextItem = chairFunction.MoveToNextAgendaItem();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return nextItem.ToDto();
        }
    }
}