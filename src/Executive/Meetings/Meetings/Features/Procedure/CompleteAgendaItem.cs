using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record CompleteAgendaItem(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(
        IApplicationDbContext context,
        IUserContext userContext,
        IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext,
        IHubContext<SecretaryHub, ISecretaryHubClient> secretaryHubContext) : IRequestHandler<CompleteAgendaItem, Result>
    {
        public async Task<Result> Handle(CompleteAgendaItem request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Minutes)
                    .ThenInclude(m => m.Items)
                .Include(x => x.Agenda!)
                    .ThenInclude(a => a.Items)
                        .ThenInclude(i => i.Voting)
                            .ThenInclude(v => v.Votes)
                .Include(x => x.Agenda!)
                    .ThenInclude(a => a.Items)
                        .ThenInclude(i => i.Election)
                            .ThenInclude(e => e.Candidates)
                .Include(x => x.Agenda!)
                    .ThenInclude(a => a.Items)
                        .ThenInclude(i => i.Election)
                            .ThenInclude(e => e.Ballots)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.Type == AgendaItemType.CallToOrder || agendaItem.Type == AgendaItemType.Election)
            {
                agendaItem.Complete();
            }
            else
            {
                var chairFunction = meeting.GetChairpersonFunction(attendee);

                if (chairFunction is null)
                {
                    return Errors.Meetings.OnlyChairpersonCanCompleteAgendaItem;
                }

                chairFunction.CompleteAgendaItem(agendaItem);
            }

            var minutesItem = await context.RecordAgendaItemMinutesAsync(meeting, agendaItem, cancellationToken);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
               .OnAgendaItemStateChanged(agendaItem.Id, (Dtos.AgendaItemState)agendaItem.State, (Dtos.AgendaItemPhase)agendaItem.Phase);

            if (minutesItem is not null)
            {
                await secretaryHubContext.Clients
                    .Group($"meeting-{meeting.Id}")
                    .OnMinutesItemChanged(minutesItem.Id);
            }

            return Result.Success;
        }
    }
}