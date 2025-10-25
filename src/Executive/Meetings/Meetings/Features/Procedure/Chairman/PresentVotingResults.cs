using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record PresentVotingResults(string OrganizationId, int Id, VotingResultsPresentationOptions Options) : IRequest<Result<string>>;

public sealed class PresentVotingResultsHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
    : IRequestHandler<PresentVotingResults, Result<string>>
{
    public async Task<Result<string>> Handle(PresentVotingResults request, CancellationToken cancellationToken)
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

        if (agendaItem.Type.Id != AgendaItemType.Voting.Id)
        {
            return Errors.Meetings.NoOngoingVoting;
        }

        if (agendaItem.Voting is null || agendaItem.Voting.State != VotingState.ResultReady)
        {
            return Errors.Meetings.VotingResultsNotReady;
        }

        await hubContext.Clients
            .Group($"meeting-{meeting.Id}")
            .OnVotingResultsPresented(agendaItem.Id, request.Options);

        return Result.SuccessWith(agendaItem.Id);
    }
}
