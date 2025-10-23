using YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings;

public interface IMeetingsProcedureHubClient : IVotingHubClient, IElectionsHubClient, IDiscussionsHubClient
{
    Task OnMeetingStateChanged(MeetingState state, string? adjournmentMessage);

    Task OnAgendaUpdated();

    Task OnAgendaItemChanged(string agendaItemId);
    Task OnAgendaItemStateChanged(string agendaItemId, AgendaItemState state);
}