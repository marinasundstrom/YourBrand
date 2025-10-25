using System.Threading.Tasks;
using YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings;

public interface IElectionsHubClient
{
    Task OnElectionStatusChanged(ElectionState state);

    Task OnElectionResultsPresented(string agendaItemId, ElectionResultsPresentationOptions options);
}
