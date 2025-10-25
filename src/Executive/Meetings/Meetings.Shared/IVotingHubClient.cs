using System.Threading.Tasks;
using YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings;

public interface IVotingHubClient
{
    Task OnVotingStatusChanged(VotingState state);

    Task OnVotingResultsPresented(string agendaItemId, VotingResultsPresentationOptions options);
}
