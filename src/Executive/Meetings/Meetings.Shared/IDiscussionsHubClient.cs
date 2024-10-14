namespace YourBrand.Meetings;

public interface IDiscussionsHubClient
{
    Task OnSpeakerRequestRevoked(string agendaItemId, string id);
    Task OnSpeakerRequestAdded(string agendaItemId, string id, string participantId);
}