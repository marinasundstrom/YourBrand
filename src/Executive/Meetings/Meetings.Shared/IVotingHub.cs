using System.Threading.Tasks;

namespace YourBrand.Meetings;

public interface IVotingHub
{
    Task PresentVotingResults(VotingResultsPresentationOptions options);
}
