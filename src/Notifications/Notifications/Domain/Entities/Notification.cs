using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Notifications.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Notifications.Domain.Entities;

public class Notification : AuditableEntity, IHasTenant, ISoftDeletable
{
    public Notification()
    {

    }

    public Notification(string title, string text)
    {
        Id = Guid.NewGuid().ToString();
        Content = text;
    }

    public string Id { get; set; } = null!;

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

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
}