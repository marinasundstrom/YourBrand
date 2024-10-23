using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaState
{
    InDraft,       // Agenda is being drafted and can still be modified.
    Finalized,     // Agenda content is finalized but not yet approved.
    Published      // Agenda has been sent or made available to all attendees.
}

public enum ApprovalStatus
{
    Pending,           // Agenda has not been approved yet.
    Approved,          // Agenda has been approved.
    Rejected           // Agenda has been rejected.
}

public class Agenda : AggregateRoot<AgendaId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<AgendaItem> _items = new HashSet<AgendaItem>();

    protected Agenda()
    {
    }

    public Agenda(AgendaId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _items = new HashSet<AgendaItem>();
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId MeetingId { get; set; }
    public MinutesId? MinutesId { get; set; }

    public AgendaState State { get; private set; } = AgendaState.InDraft;
    public ApprovalStatus ApprovalStatus { get; private set; } = ApprovalStatus.Pending;

    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    public void FinalizeContent()
    {
        if (State != AgendaState.InDraft)
        {
            throw new InvalidOperationException("Agenda can only be finalized from the draft state.");
        }

        State = AgendaState.Finalized;
    }

    public void SubmitForApproval()
    {
        if (State != AgendaState.Finalized)
        {
            throw new InvalidOperationException("Agenda must be finalized before submitting for approval.");
        }

        if (ApprovalStatus != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Agenda is already submitted for approval.");
        }

        ApprovalStatus = ApprovalStatus.Pending;
    }

    public void Approve()
    {
        if (ApprovalStatus != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Agenda is not pending approval.");
        }

        ApprovalStatus = ApprovalStatus.Approved;
        ApprovedAt = DateTimeOffset.UtcNow;
    }

    public void Reject()
    {
        if (ApprovalStatus != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Agenda is not pending approval.");
        }

        ApprovalStatus = ApprovalStatus.Rejected;
        RejectedAt = DateTimeOffset.UtcNow;
    }

    public void Publish()
    {
        if (State != AgendaState.Finalized)
        {
            throw new InvalidOperationException("Agenda must be finalized before publishing.");
        }

        if (ApprovalStatus != ApprovalStatus.Approved)
        {
            throw new InvalidOperationException("Agenda must be approved before publishing.");
        }

        State = AgendaState.Published;
        PublishedAt = DateTimeOffset.UtcNow;
    }

    public IReadOnlyCollection<AgendaItem> Items => _items;

    public AgendaItem AddItem(AgendaItemType type, string title, string description)
    {
        if (_items.Any(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("An agenda item with the same title already exists.");
        }

        if (State != AgendaState.InDraft && State != AgendaState.Published)
        {
            throw new InvalidOperationException("Cannot add agenda items unless the agenda is in draft or under review.");
        }

        int order = 1;

        try
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var item = new AgendaItem(type, title, description);
        item.Order = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveAgendaItem(AgendaItem item)
    {
        int i = 1;
        var r = _items.Remove(item);
        foreach (var item0 in _items)
        {
            item0.Order = i++;
        }
        return r;
    }

    public bool ReorderAgendaItem(AgendaItem agendaItem, int newOrderPosition)
    {
        if (!_items.Contains(agendaItem))
        {
            throw new InvalidOperationException("Agenda item does not exist in this agenda.");
        }

        if (newOrderPosition < 1 || newOrderPosition > _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newOrderPosition), "New order position is out of range.");
        }

        int oldOrderPosition = agendaItem.Order;

        if (oldOrderPosition == newOrderPosition)
            return false;

        // Flyttar objektet uppåt i listan
        if (newOrderPosition < oldOrderPosition)
        {
            var itemsToIncrement = Items
                .Where(i => i.Order >= newOrderPosition && i.Order < oldOrderPosition)
                .ToList();

            foreach (var item in itemsToIncrement)
            {
                item.Order += 1;
            }
        }
        // Flyttar objektet nedåt i listan
        else
        {
            var itemsToDecrement = Items
                .Where(i => i.Order > oldOrderPosition && i.Order <= newOrderPosition)
                .ToList();

            foreach (var item in itemsToDecrement)
            {
                item.Order -= 1;
            }
        }

        // Uppdatera order för objektet som flyttas
        agendaItem.Order = newOrderPosition;

        return true;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}