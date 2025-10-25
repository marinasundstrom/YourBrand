using System;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Functions;

public sealed class ChairpersonMeetingFunction
{
    private readonly Meeting _meeting;
    private readonly MeetingAttendee _attendee;

    internal ChairpersonMeetingFunction(Meeting meeting, MeetingAttendee attendee)
    {
        _meeting = meeting;
        _attendee = attendee;
    }

    public Meeting Meeting => _meeting;

    public MeetingAttendee Attendee => _attendee;

    public void StartMeeting() => _meeting.StartMeeting();

    public void ResumeMeeting() => _meeting.ResumeMeeting();

    public void AdjournMeeting(string message) => _meeting.AdjournMeeting(message);

    public void CancelMeeting() => _meeting.CancelMeeting();

    public void EndMeeting() => _meeting.EndMeeting();

    public void ResetMeetingProgress() => _meeting.ResetMeetingProgress();

    public AgendaItem MoveToNextAgendaItem() => _meeting.MoveToNextAgendaItem();

    public void CancelAgendaItem(AgendaItem agendaItem) => agendaItem.Cancel();

    public void CompleteAgendaItem(AgendaItem agendaItem) => agendaItem.Complete();

    public void PostponeAgendaItem(AgendaItem agendaItem) => agendaItem.Postpone();

    public void StartDiscussion(AgendaItem agendaItem)
    {
        agendaItem.StartDiscussion();
        _meeting.NotifyAgendaItemStateChanged(agendaItem);
    }

    public void EndDiscussion(AgendaItem agendaItem)
    {
        agendaItem.EndDiscussion();
        _meeting.NotifyAgendaItemStateChanged(agendaItem);
    }

    public void StartVoting(AgendaItem agendaItem)
    {
        agendaItem.StartVoting();
        _meeting.NotifyAgendaItemStateChanged(agendaItem);
    }

    public void EndVoting(AgendaItem agendaItem)
    {
        agendaItem.EndVoting();
        _meeting.NotifyAgendaItemStateChanged(agendaItem);
    }

    public void StartElection(AgendaItem agendaItem) => agendaItem.StartElection();

    public void EndElection(AgendaItem agendaItem) => agendaItem.EndElection();

    public void SetDiscussionSpeakingTime(AgendaItem agendaItem, TimeSpan? speakingTimeLimit)
    {
        agendaItem.Discussion ??= new Discussion
        {
            OrganizationId = _meeting.OrganizationId,
            TenantId = _meeting.TenantId,
            AgendaItemId = agendaItem.Id
        };

        agendaItem.Discussion.SetSpeakingTimeLimit(speakingTimeLimit);
    }

    public void ExtendSpeakerTime(AgendaItem agendaItem, string speakerRequestId, TimeSpan additionalTime)
    {
        if (agendaItem.Discussion is null)
        {
            throw new InvalidOperationException("No ongoing discussion to extend speaker time for.");
        }

        agendaItem.Discussion.ExtendSpeakerTime(new SpeakerRequestId(speakerRequestId), additionalTime);
    }

    public SpeakerClockSnapshot StartSpeakerClock(AgendaItem agendaItem, DateTimeOffset now)
    {
        if (agendaItem.Discussion is null)
        {
            throw new InvalidOperationException("No ongoing discussion to start the speaker clock for.");
        }

        agendaItem.Discussion.StartCurrentSpeakerClock(now);
        return agendaItem.Discussion.GetCurrentSpeakerClockSnapshot(now);
    }

    public SpeakerClockSnapshot StopSpeakerClock(AgendaItem agendaItem, DateTimeOffset now)
    {
        if (agendaItem.Discussion is null)
        {
            throw new InvalidOperationException("No ongoing discussion to stop the speaker clock for.");
        }

        agendaItem.Discussion.StopCurrentSpeakerClock(now);
        return agendaItem.Discussion.GetCurrentSpeakerClockSnapshot(now);
    }

    public void ResetSpeakerClock(AgendaItem agendaItem)
    {
        if (agendaItem.Discussion is null)
        {
            throw new InvalidOperationException("No ongoing discussion to reset the speaker clock for.");
        }

        agendaItem.Discussion.ResetCurrentSpeakerClock();
    }

    public SpeakerTransition MoveToNextSpeaker(AgendaItem agendaItem)
    {
        if (agendaItem.Discussion is null)
        {
            throw new InvalidOperationException("No ongoing discussion to move to the next speaker for.");
        }

        return agendaItem.Discussion.MoveToNextSpeaker();
    }
}
