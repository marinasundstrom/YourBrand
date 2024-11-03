
using System.Text.Json;

using YourBrand.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public sealed class Widget : AuditableEntity<Guid>, IHasTenant
{
    private Widget()
    {

    }

    public Widget(string widgetId, string? userId) : base(Guid.NewGuid())
    {
        WidgetId = widgetId;
        UserId = userId;
    }

    public TenantId TenantId { get; set; }

    public string WidgetId { get; set; } = null!;

    public string WidgetAreaId { get; set; } = default!;

    public string? UserId { get; set; } = null!;

    public JsonDocument? Settings { get; set; } = null!;

    /*
    public User CreatedBy { get; set; } = null!;

    public UserId CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
    */
}


public sealed class WidgetArea : AuditableEntity<string>
{
    readonly HashSet<Widget> widgets = new HashSet<Widget>();

    private WidgetArea()
    {

    }

    public WidgetArea(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public IReadOnlyCollection<Widget> Widgets => widgets;

    public void AddWidget(Widget widget)
    {
        widgets.Add(widget);
    }

    public void RemoveWidget(Widget widget)
    {
        widgets.Remove(widget);
    }
}