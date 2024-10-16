using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Minutes.MinutesDetails;

public class MinutesItemViewModel
{
    public string Id { get; set; }

    public int Order { get; set; }

    [Required]
    public AgendaItemType Type { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public int AgendaId { get; set; }

    public string AgendaItemId { get; set; }

    public MinutesItemViewModel Clone()
    {
        return new MinutesItemViewModel
        {
            Id = Id,
            Order = Order,
            Type = Type,
            Title = Title,
            Description = Description,
            AgendaId = AgendaId,
            AgendaItemId = AgendaItemId
        };
    }
}