using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaState
{
    InDraft,           // Agenda is being drafted and can still be modified.
    UnderReview,       // Agenda is under review by attendees or stakeholders.
    Approved,          // Agenda is finalized and approved for the meeting.
    Published,         // Agenda has been sent or made available to all attendees.
    InProgress,        // The meeting is happening, and the agenda is being followed.
    Completed,         // The meeting is finished, and all agenda items have been covered.
    InFollowUp         // Post-meeting follow-up tasks or action items are in progress.
}

public enum ApprovalStatus
{
    Pending,           // Agenda has not been approved yet.
    Approved,          // Agenda has been approved.
    Rejected           // Agenda has been rejected.
}

public enum CompletionStatus
{
    Incomplete,        // Agenda has not yet been completed.
    Completed          // Agenda has been fully completed.
}

public class Agenda : AggregateRoot<AgendaId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<AgendaItem> _items = new HashSet<AgendaItem>();

    protected Agenda()
    {
    }

    public Agenda(AgendaId id)
    {
        Id = id;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId MeetingId { get; set; }
    public MinutesId? MinutesId { get; set; }

    public AgendaState State { get; set;} = AgendaState.InDraft;

    public ApprovalStatus ApprovalStatus { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public bool? WasAdjusted { get; set; }
    public DateTimeOffset? AdjustedAt { get; set; }
    public bool? CompletedAt { get; set; }

    public IReadOnlyCollection<AgendaItem> Items => _items;

    public AgendaItem AddAgendaItem(AgendaItemType type, string title, string description) 
    {
        int order = 1;

        try 
        {
            var last = _items.OrderByDescending(x => x.Order ).First();
            order = last.Order  + 1;
        }
        catch {}

        var item = new AgendaItem(type, title, description);
        item.Order  = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveAgendaItem(AgendaItem item)
    {
        return _items.Remove(item);
    }

    public bool ReorderAgendaItem(AgendaItem agendaItem, int newOrderPosition)
    {
        int oldOrderPosition = agendaItem.Order ;

        if (oldOrderPosition == newOrderPosition)
            return false;

        // Flyttar objektet uppåt i listan
        if (newOrderPosition < oldOrderPosition)
        {
            var itemsToIncrement = Items
                .Where(i => i.Order  >= newOrderPosition && i.Order  < oldOrderPosition)
                .ToList();

            foreach (var item in itemsToIncrement)
            {
                item.Order  += 1;
            }
        }
        // Flyttar objektet nedåt i listan
        else
        {
            var itemsToDecrement = Items
                .Where(i => i.Order  > oldOrderPosition && i.Order  <= newOrderPosition)
                .ToList();

            foreach (var item in itemsToDecrement)
            {
                item.Order  -= 1;
            }
        }

        // Uppdatera order för objektet som flyttas
        agendaItem.Order  = newOrderPosition;

        return true;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
