using System;

namespace YourBrand.Meetings;

public interface IDiscussionsHubClient
{
    Task OnDiscussionStatusChanged(int status);

    Task OnMovedToNextSpeaker(string agendaItemId, string? id);

    Task OnSpeakerRequestRevoked(string agendaItemId, string id);
    Task OnSpeakerRequestAdded(string agendaItemId, string id, string attendeeId, string name);

    Task OnSpeakerTimeExtended(string agendaItemId, string speakerRequestId, int? allocatedSeconds);
    Task OnDiscussionSpeakingTimeChanged(string agendaItemId, int? speakingTimeLimitSeconds);
    Task OnSpeakerClockStarted(string agendaItemId, string speakerRequestId, int elapsedSeconds, DateTimeOffset startedAtUtc);
    Task OnSpeakerClockStopped(string agendaItemId, string speakerRequestId, int elapsedSeconds);
    Task OnSpeakerClockReset(string agendaItemId, string speakerRequestId, int elapsedSeconds);
}
