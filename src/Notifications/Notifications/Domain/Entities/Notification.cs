using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Notifications.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Notifications.Domain.Entities;

public class Notification : AuditableEntity<string>, IHasTenant, ISoftDeletable
{
    public Notification()
    {

    }

    public Notification(string id) : base(id)
    {

    }

    public Notification(string title, string text) : base(Guid.NewGuid().ToString())
    {
        Content = text;
    }

    public TenantId TenantId { get; set; }

    public string? Content { get; set; }

    public string? Tag { get; set; }

    public string? Link { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? Read { get; set; }

    public string? ItemId { get; set; }

    public string? UserId { get; set; }

    public DateTimeOffset? Published { get; set; }

    public DateTimeOffset? ScheduledFor { get; set; }

    public string? ScheduledJobId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
}