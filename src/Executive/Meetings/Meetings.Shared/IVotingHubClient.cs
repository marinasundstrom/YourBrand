using YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings;

public interface IVotingHubClient
{
    Task OnVotingStatusChanged(VotingState state);
}
