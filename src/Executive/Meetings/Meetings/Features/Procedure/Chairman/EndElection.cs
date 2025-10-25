using System.Linq;
using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record EndElection(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<EndElection, Result>
    {
        public async Task<Result> Handle(EndElection request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Attendees)
                .ThenInclude(x => x.Functions)
                .ThenInclude(x => x.Function)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
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

            var chairFunction = meeting.GetChairpersonFunction(attendee);

            if (chairFunction is null)
            {
                return Errors.Meetings.OnlyChairpersonCanEndVoting;
            }

            chairFunction.EndElection(agendaItem);

            var election = agendaItem.Election;

            if (election?.MeetingFunctionId is int meetingFunctionId &&
                election.ElectedCandidate?.AttendeeId is { } electedAttendeeId)
            {
                var winningAttendee = meeting.Attendees
                    .FirstOrDefault(x => x.Id == electedAttendeeId);

                if (winningAttendee is not null)
                {
                    var meetingFunction = await context.MeetingFunctions
                        .FirstOrDefaultAsync(x => x.Id == meetingFunctionId, cancellationToken);

                    if (meetingFunction is not null)
                    {
                        meeting.AssignFunctionToAttendee(winningAttendee, meetingFunction);
                    }
                }
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}