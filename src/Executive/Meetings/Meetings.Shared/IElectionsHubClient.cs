namespace YourBrand.Meetings;

public interface IElectionsHubClient
{
    Task OnElectionStatusChanged(int status);
}
