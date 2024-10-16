using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MeetingState
{
    Draft,
    Scheduled,
    InProgress,
    Completed,
    Canceled
}

public enum AttendeeAccessLevel
{
    Everyone,
    Participants,
    Select
}

public class Meeting : AggregateRoot<MeetingId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<MeetingAttendee> _attendees = new HashSet<MeetingAttendee>();

    protected Meeting()
    {
    }

    public Meeting(MeetingId id, string title)
    {
        Id = id;
        Title = title;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Title { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public string Location { get; set; }
    public string? Description { get; set; }
    public MeetingState State { get; set; } = MeetingState.Draft;
    public IReadOnlyCollection<MeetingAttendee> Attendees => _attendees;
    public Agenda? Agenda { get; set; }
    public int? CurrentAgendaItemIndex { get; private set; } = null;
    public Quorum Quorum { get; set; } = new Quorum();

    public AttendeeAccessLevel SpeakingRightsAccessLevel { get;} = AttendeeAccessLevel.Participants;
    public AttendeeAccessLevel VotingRightsAccessLevel { get; } = AttendeeAccessLevel.Participants;

    public Minutes? Minutes { get; set; }

    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset? Canceled { get; set; }
    public DateTimeOffset? Ended { get; set; }


    public bool IsQuorumMet()
    {
        int presentAttendees = 0;

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Everyone) 
        {
            presentAttendees = Attendees.Count(p => p.IsPresent);
        }
        else 
        {
            presentAttendees = Attendees.Count(p => p.IsPresent && p.HasVotingRights.GetValueOrDefault());
        }
        return presentAttendees >= Quorum.RequiredNumber;
    }

    public void StartMeeting()
    {
        if (State != MeetingState.Scheduled)
        {
            throw new InvalidOperationException("Meeting cannot be started.");
        }

        if (!IsQuorumMet())
        {
            throw new InvalidOperationException("Quorum not met.");
        }

        Started = DateTimeOffset.UtcNow;

        State = MeetingState.InProgress;

        CurrentAgendaItemIndex = 0;

        //var agendaItems = Agenda.Items.OrderBy(ai => ai.Order).ToList();

        //agendaItems[CurrentAgendaItemIndex.GetValueOrDefault()].State = AgendaItemState.UnderDiscussion;
    }

    public void CancelMeeting()
    {
        /*
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }
        */

        Canceled = DateTimeOffset.UtcNow;

        State = MeetingState.Canceled;

        var agendaItems = Agenda.Items.OrderBy(ai => ai.Order ).ToList();

        foreach(var item in agendaItems.Skip(CurrentAgendaItemIndex.GetValueOrDefault())) 
        {
            item.Cancel();
        }

        CurrentAgendaItemIndex = null;
    }

    public void EndMeeting()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        Ended = DateTimeOffset.UtcNow;

        State = MeetingState.Completed;

        CurrentAgendaItemIndex = null;
    }

    public AgendaItem MoveToNextAgendaItem()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        var agendaItems = Agenda?.Items.OrderBy(ai => ai.Order ).ToList();
        if (agendaItems == null || CurrentAgendaItemIndex >= agendaItems.Count - 1)
        {
            throw new InvalidOperationException("No more agenda items.");
        }

        var currentItem = agendaItems.ElementAt(CurrentAgendaItemIndex.GetValueOrDefault());

        if (currentItem.State == AgendaItemState.Pending)
        {
            throw new InvalidOperationException("Can't skip item that is in pending state.");
        }

        CurrentAgendaItemIndex++;

        var nextItem = agendaItems.ElementAt(CurrentAgendaItemIndex.GetValueOrDefault());

        return nextItem;
    }

    public AgendaItem? GetCurrentAgendaItem()
    {
        if(CurrentAgendaItemIndex is null) 
        {
            return null;
        }

        return Agenda?.Items
            .OrderBy(ai => ai.Order )
            .ElementAtOrDefault(CurrentAgendaItemIndex.GetValueOrDefault());
    }

    public AgendaItem? GetAgendaItem(string id)
    {
        return Agenda?.Items.FirstOrDefault(x => x.Id == id);
    }

    public MeetingAttendee AddAttendee(string name, string? userId, string email, AttendeeRole role, bool? hasSpeakingRights, bool? hasVotingRights, 
        MeetingGroupId? meetingGroupId = null, MeetingGroupMemberId? meetingGroupMemberId = null)
    {
        int order = 1;

        try
        {
            var last = _attendees.OrderByDescending(x => x.Order ).First();
            order = last.Order  + 1;
        }
        catch { }

        var attendee = new MeetingAttendee
        {
            OrganizationId = OrganizationId,
            MeetingId = Id,
            Name = name,
            UserId = userId,
            Email = email,
            Role = role,
            HasSpeakingRights = hasSpeakingRights,
            HasVotingRights = hasVotingRights,
            MeetingGroupId = meetingGroupId,
            MeetingGroupMemberId = meetingGroupMemberId
        };
        attendee.Order  = order;

        _attendees.Add(attendee);

        return attendee;
    }

    public bool RemoveAttendee(MeetingAttendee attendee)
    {
        return _attendees.Remove(attendee);
    }

    public MeetingAttendee? GetAttendeeById(string id)
    {
        return Attendees.FirstOrDefault(x => x.Id == id);
    }

    public MeetingAttendee? GetAttendeeByUserId(string userId) 
    {
        return Attendees.FirstOrDefault(x => x.UserId == userId);
    }

    public void ResetMeetingProgress()
    {
        State = MeetingState.Scheduled;

        CurrentAgendaItemIndex = null;

        foreach (var agendaItem in Agenda!.Items) 
        {
            agendaItem.State = AgendaItemState.Pending;
        }
    }

    public bool IsAttendeeAllowedToSpeak(MeetingAttendee attendee)
    {
        if(SpeakingRightsAccessLevel == AttendeeAccessLevel.Everyone) 
        {
            return true;
        }

        if (SpeakingRightsAccessLevel == AttendeeAccessLevel.Participants && attendee.Role != AttendeeRole.Observer)
        {
            if (attendee.HasSpeakingRights is null)
            {
                return true;
            }

            return attendee.HasSpeakingRights.GetValueOrDefault();
        }

        if(SpeakingRightsAccessLevel == AttendeeAccessLevel.Select) 
        {
            return attendee.HasSpeakingRights.GetValueOrDefault();
        }

        return false;
    }

    public bool IsAttendeeAllowedToVote(MeetingAttendee attendee)
    {
        if(VotingRightsAccessLevel == AttendeeAccessLevel.Everyone) 
        {
            return true;
        }

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Participants && attendee.Role != AttendeeRole.Observer)
        {
            if(attendee.HasVotingRights is null) 
            {
                return true;
            }

            return attendee.HasVotingRights.GetValueOrDefault();
        }

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Select)
        {
            return attendee.HasVotingRights.GetValueOrDefault();
        }

        return false;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}