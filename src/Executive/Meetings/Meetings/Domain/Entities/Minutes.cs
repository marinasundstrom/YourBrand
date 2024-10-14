using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MinutesState 
{
    Drafting,           // Minute is being drafted, can still be modified.
    Reviewing,          // Minute is under review by participants or stakeholders.
    Approved           // Minute is finalized and approved for the meeting.
}

public class Minutes : AggregateRoot<MinutesId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<MinutesItem> _items = new HashSet<MinutesItem>();

    protected Minutes()
    {
    }

    public Minutes(int id)
    {
        Id = id;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId MeetingId { get; set; }

    public MinutesState State { get; set;} = MinutesState.Drafting;
    
    /*
    public bool? IsApproved { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public bool? IsAdjusted { get; set; }
    public DateTimeOffset? AdjustedAt { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    */

    public IReadOnlyCollection<MinutesItem> Items => _items;

    public MinutesItem AddItem(MinutesItemType type, int? agendaId, string? agendaItemId, string title, string description) 
    {
        int order = 1;

        try 
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch {}

        var item = new MinutesItem(type, title, description);
        item.AgendaId = agendaId;
        item.AgendaItemId = agendaItemId;
        item.Order = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveItem(MinutesItem item)
    {
        return _items.Remove(item);
    }

    public bool MoveItem(MinutesItem agendaItem, int newOrderPosition)
    {
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
