using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MotionStatus { Proposed }

public class Motion : AggregateRoot<MotionId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<MotionItem> _items = new HashSet<MotionItem>();

    protected Motion()
    {
    }

    public Motion(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Title { get; set; }
    public string Text { get; set; }
    public MotionStatus Status { get; set; } = MotionStatus.Proposed;

    public IReadOnlyCollection<MotionItem> Items => _items;

    public MotionItem AddItem(string text)
    {
        int order = 1;

        try
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var item = new MotionItem(text);
        item.Order = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveItem(MotionItem item)
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
