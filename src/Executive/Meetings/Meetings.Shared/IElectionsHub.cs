using System.Threading.Tasks;

namespace YourBrand.Meetings;

public interface IElectionsHub
{
    Task PresentElectionResults(ElectionResultsPresentationOptions options);
}
