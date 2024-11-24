using YourBrand.Auditability;
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

public class Agenda : AggregateRoot<AgendaId>, IAuditableEntity<AgendaId>, IHasTenant, IHasOrganization
{
    readonly HashSet<AgendaItem> _items = new HashSet<AgendaItem>();

    protected Agenda() { }

    public Agenda(AgendaId id) : base(id)
    {
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

    [Throws(typeof(InvalidOperationException))]
    public void FinalizeContent()
    {
        if (State != AgendaState.InDraft)
            throw new InvalidOperationException("Agenda can only be finalized from the draft state.");

        State = AgendaState.Finalized;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Approve()
    {
        if (ApprovalStatus != ApprovalStatus.Pending)
            throw new InvalidOperationException("Agenda is not pending approval.");

        ApprovalStatus = ApprovalStatus.Approved;
        ApprovedAt = DateTimeOffset.UtcNow;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Publish()
    {
        if (State != AgendaState.Finalized)
            throw new InvalidOperationException("Agenda must be finalized before publishing.");

        if (ApprovalStatus != ApprovalStatus.Approved)
            throw new InvalidOperationException("Agenda must be approved before publishing.");

        State = AgendaState.Published;
        PublishedAt = DateTimeOffset.UtcNow;
    }

    public IReadOnlyCollection<AgendaItem> Items => _items.Where(x => x.ParentId == null).ToList();

    [Throws(typeof(InvalidOperationException))]
    public AgendaItem AddItem(AgendaItemType type, string title, string description, Election? election = null)
    {
        if (ApprovalStatus == ApprovalStatus.Approved)
            throw new InvalidOperationException("Cannot modify agenda items after approval.");

        if (_items.Any(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("An agenda item with the same title already exists.");

        int order = _items.Where(x => x.ParentId == null)
                          .OrderByDescending(x => x.Order)
                          .Select(x => x.Order)
                          .FirstOrDefault() + 1;

        var item = new AgendaItem(OrganizationId, type, title, description) { 
            AgendaId = Id, 
            Order = order, 
            Election = election 
        };
        
        _items.Add(item);
        return item;
    }

    [Throws(typeof(InvalidOperationException))]
    public bool RemoveAgendaItem(AgendaItem item)
    {
        if (ApprovalStatus == ApprovalStatus.Approved)
            throw new InvalidOperationException("Cannot remove agenda items after approval.");

        int i = 1;
        var removed = _items.Remove(item);
        foreach (var agendaItem in _items)
        {
            agendaItem.Order = i++;
        }
        return removed;
    }

    [Throws(typeof(InvalidOperationException))]
    public bool ReorderAgendaItem(AgendaItem agendaItem, int newOrderPosition)
    {
        if (ApprovalStatus == ApprovalStatus.Approved)
            throw new InvalidOperationException("Cannot reorder agenda items after approval.");

        if (!_items.Contains(agendaItem))
            throw new InvalidOperationException("Agenda item does not exist in this agenda.");

        if (newOrderPosition < 1 || newOrderPosition > _items.Count)
            throw new ArgumentOutOfRangeException(nameof(newOrderPosition), "New order position is out of range.");

        int oldOrderPosition = agendaItem.Order;
        if (oldOrderPosition == newOrderPosition)
            return false;

        var itemsToShift = oldOrderPosition < newOrderPosition
            ? Items.Where(i => i.Order > oldOrderPosition && i.Order <= newOrderPosition)
            : Items.Where(i => i.Order >= newOrderPosition && i.Order < oldOrderPosition);

        foreach (var item in itemsToShift)
        {
            item.Order += oldOrderPosition < newOrderPosition ? -1 : 1;
        }

        agendaItem.Order = newOrderPosition;
        return true;
    }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}