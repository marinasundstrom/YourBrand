using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaState 
{
    Drafting,           // Agenda is being drafted, can still be modified.
    Reviewing,          // Agenda is under review by participants or stakeholders.
    Approved,           // Agenda is finalized and approved for the meeting.
    Distributed,        // Agenda has been sent to all participants.
    InProgress,         // The meeting is happening, and the agenda is being followed.
    //Adjusted,           // The agenda has been adjusted during the meeting.
    Completed,          // The meeting is finished, and all agenda items have been covered.
    FollowUp            // Post-meeting follow-up tasks or action items are in progress.
}

public class Agenda : AggregateRoot<AgendaId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<AgendaItem> _items = new HashSet<AgendaItem>();

    protected Agenda()
    {
    }

    public Agenda(int id)
    {
        Id = id;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId MeetingId { get; set; }

    public AgendaState State { get; set;} = AgendaState.Drafting;

    /*
    public bool? IsApproved { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public bool? IsAdjusted { get; set; }
    public DateTimeOffset? AdjustedAt { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    */

    public IReadOnlyCollection<AgendaItem> Items => _items;

    public AgendaItem AddItem(AgendaItemType type, string title, string description) 
    {
        int order = 1;

        try 
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch {}

        var item = new AgendaItem(type, title, description);
        item.Order = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveItem(AgendaItem item)
    {
        return _items.Remove(item);
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
