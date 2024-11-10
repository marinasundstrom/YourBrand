namespace YourBrand.Meetings;

public interface IVotingHubClient
{
    Task OnVotingStatusChanged(int status);
}
