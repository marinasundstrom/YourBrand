namespace YourBrand.Meetings;

public interface IMeetingsProcedureHubClient
{
    Task OnMeetingStateChanged();

    Task OnAgendaItemChanged(string agendaItemId);

    Task OnAgendaItemStatusChanged(string agendaItemId);
}