namespace YourBrand.Meetings;

public interface IDiscussionsHub
{
    Task MoveToNextSpeaker();

    Task RequestSpeakerTime(string agendaItemId);
    Task RevokeSpeakerTime(string agendaItemId);

    Task SetDiscussionSpeakingTime(string agendaItemId, int? speakingTimeLimitSeconds);
    Task ExtendSpeakerTime(string agendaItemId, string speakerRequestId, int additionalSeconds);
    Task StartCurrentSpeakerClock(string agendaItemId);
    Task StopCurrentSpeakerClock(string agendaItemId);
    Task ResetCurrentSpeakerClock(string agendaItemId);
}