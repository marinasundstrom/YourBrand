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
    Cancelled
}

public class Meeting : AggregateRoot<MeetingId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<MeetingParticipant> _participants = new HashSet<MeetingParticipant>();

    protected Meeting()
    {
    }

    public Meeting(int id, string title)
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
    public IReadOnlyCollection<MeetingParticipant> Participants => _participants;
    public Agenda? Agenda { get; set; }
    public int? CurrentAgendaItemIndex { get; private set; } = null;
    public Quorum Quorum { get; set; } = new Quorum();
    public Minutes? Minutes { get; set; }

    public bool IsQuorumMet()
    {
        int presentParticipants = Participants.Count(p => p.IsPresent && p.HasVotingRights);
        return presentParticipants >= Quorum.RequiredNumber;
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

        State = MeetingState.InProgress;

        CurrentAgendaItemIndex = 0;

        //var agendaItems = Agenda.Items.OrderBy(ai => ai.Order).ToList();

        //agendaItems[CurrentAgendaItemIndex.GetValueOrDefault()].State = AgendaItemState.UnderDiscussion;
    }

    public void EndMeeting()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        State = MeetingState.Completed;

        CurrentAgendaItemIndex = null;
    }

    public AgendaItem MoveToNextAgendaItem()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        var agendaItems = Agenda?.Items.OrderBy(ai => ai.Order).ToList();
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
            .OrderBy(ai => ai.Order)
            .ElementAtOrDefault(CurrentAgendaItemIndex.GetValueOrDefault());
    }

    public AgendaItem? GetAgendaItem(string id)
    {
        return Agenda?.Items.FirstOrDefault(x => x.Id == id);
    }

    public MeetingParticipant AddParticipant(string name, string? userId, string email, ParticipantRole role, bool HasVotingRights)
    {
        var participant = new MeetingParticipant
        {
            OrganizationId = OrganizationId,
            MeetingId = Id,
            Name = name,
            UserId = userId,
            Email = email,
            Role = role,
            HasVotingRights = HasVotingRights
        };

        _participants.Add(participant);

        return participant;
    }

    public bool RemoveParticipant(MeetingParticipant participant)
    {
        return _participants.Remove(participant);
    }

    public void ResetProcedure()
    {
        State = MeetingState.Scheduled;

        CurrentAgendaItemIndex = null;

        foreach (var agendaItem in Agenda!.Items) 
        {
            agendaItem.State = AgendaItemState.Pending;
        }
    }

    // ...

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}