namespace YourBrand.Meetings;

public interface IMeetingsProcedureHubClient
{
    Task OnMeetingStateChanged();

    Task OnAgendaUpdated();

    Task OnAgendaItemChanged(string agendaItemId);
    Task OnAgendaItemStatusChanged(string agendaItemId);

    Task OnSpeakerRequestRevoked(string id);
    Task OnSpeakerRequestAdded(string id);
}