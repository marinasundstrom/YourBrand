namespace YourBrand.Meetings;

public interface ISecretaryHubClient
{
    Task OnMinutesUpdated();

    Task OnMinutesItemChanged(string minutesItemId);
    Task OnMinutesItemStatusChanged(string minutesItemId);
}