namespace YourBrand.Meetings;

public interface IDiscussionsHub
{
    Task RequestSpeakerTime(string agendaItemId);
    Task RevokeSpeakerTime(string agendaItemId);
}