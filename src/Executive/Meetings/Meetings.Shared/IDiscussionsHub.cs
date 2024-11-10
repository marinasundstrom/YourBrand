namespace YourBrand.Meetings;

public interface IDiscussionsHub
{
    Task MoveToNextSpeaker();

    Task RequestSpeakerTime(string agendaItemId);
    Task RevokeSpeakerTime(string agendaItemId);
}