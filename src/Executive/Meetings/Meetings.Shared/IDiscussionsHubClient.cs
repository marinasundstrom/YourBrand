namespace YourBrand.Meetings;

public interface IDiscussionsHubClient
{
    Task OnSpeakerRequestRevoked(string id);
    Task OnSpeakerRequestAdded(string id, string participantId);
}