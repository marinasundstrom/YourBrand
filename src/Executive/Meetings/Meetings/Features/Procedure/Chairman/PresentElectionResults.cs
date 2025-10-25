using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record PresentElectionResults(string OrganizationId, int Id, ElectionResultsPresentationOptions Options) : IRequest<Result<string>>;

public sealed class PresentElectionResultsHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
    : IRequestHandler<PresentElectionResults, Result<string>>
{
    public async Task<Result<string>> Handle(PresentElectionResults request, CancellationToken cancellationToken)
    {
        var meeting = await context.Meetings
            .InOrganization(request.OrganizationId)
            .Include(x => x.Agenda)
            .ThenInclude(x => x.Items.OrderBy(x => x.Order))
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (meeting is null)
        {
            return Errors.Meetings.MeetingNotFound;
        }

        var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

        if (attendee is null)
        {
            return Errors.Meetings.YouAreNotAttendeeOfMeeting;
        }

        var chairFunction = meeting.GetChairpersonFunction(attendee);

        if (chairFunction is null)
        {
            return Errors.Meetings.OnlyChairpersonCanPresentResults;
        }

        var agendaItem = meeting.GetCurrentAgendaItem();

        if (agendaItem is null)
        {
            return Errors.Meetings.NoActiveAgendaItem;
        }

        if (agendaItem.Type.Id != AgendaItemType.Election.Id)
        {
            return Errors.Meetings.NoOngoingElection;
        }

        if (agendaItem.Election is null || agendaItem.Election.State != ElectionState.ResultReady)
        {
            return Errors.Meetings.ElectionResultsNotReady;
        }

        await hubContext.Clients
            .Group($"meeting-{meeting.Id}")
            .OnElectionResultsPresented(agendaItem.Id, request.Options);

        return Result.SuccessWith<string>(agendaItem.Id);
    }
}
