using YourBrand.Meetings;

namespace YourBrand.Meetings.Minutes.Secretary;

public class SecretaryMinutesItemViewModel
{
    public string Id { get; set; } = null!;

    public int Order { get; set; }

    public AgendaItemType Type { get; set; } = default!;

    public string Title { get; set; } = string.Empty;

    public MinutesItemState State { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ServerDescription { get; private set; } = string.Empty;

    public int? AgendaId { get; set; }

    public string? AgendaItemId { get; set; }

    public int? MotionId { get; set; }

    public bool IsSaving { get; set; }

    public bool HasChanges => Description != ServerDescription;

    public void UpdateFrom(MinutesItem item)
    {
        Order = item.Order;
        Type = item.Type;
        Title = item.Title ?? string.Empty;
        State = item.State;
        AgendaId = item.AgendaId;
        AgendaItemId = item.AgendaItemId;
        MotionId = item.MotionId;

        ApplyServerDescription(item.Description ?? string.Empty);
    }

    public void ApplyServerDescription(string description)
    {
        var normalized = description ?? string.Empty;
        var hadChanges = Description != ServerDescription;

        ServerDescription = normalized;

        if (!hadChanges)
        {
            Description = normalized;
        }
    }

    public void AcceptChanges()
    {
        ServerDescription = Description;
    }

    public void ResetChanges()
    {
        Description = ServerDescription;
    }
}
