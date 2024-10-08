using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

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

    public IReadOnlyCollection<AgendaItem> Items => _items;

    public AgendaItem AddItem(string title, string description) 
    {
        int order = 1;

        try 
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch {}

        var item = new AgendaItem(title, description);
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
