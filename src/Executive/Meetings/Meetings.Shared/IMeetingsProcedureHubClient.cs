namespace YourBrand.Meetings;

public interface IMeetingsProcedureHubClient
{
    Task OnMeetingStateChanged();

    Task OnAgendaUpdated();

    Task OnAgendaItemChanged(string agendaItemId);
    Task OnAgendaItemStatusChanged(string agendaItemId);
}