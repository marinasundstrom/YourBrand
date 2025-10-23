namespace YourBrand.Meetings;

public interface IDiscussionsHubClient
{
    Task OnDiscussionStatusChanged(int status);

    Task OnMovedToNextSpeaker(string agendaItemId, string? id);

    Task OnSpeakerRequestRevoked(string agendaItemId, string id);
    Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId, string name);
}
