using YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings;

public interface IElectionsHubClient
{
    Task OnElectionStatusChanged(ElectionState state);
}
