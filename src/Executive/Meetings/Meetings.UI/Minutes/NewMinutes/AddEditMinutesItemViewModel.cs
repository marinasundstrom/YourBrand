using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.Minutes.NewMinutes;

public class AddEditMinutesItemViewModel
{
    public int Order { get; set; }

    [Required]
    public MinutesItemType Type { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public int AgendaId { get; set; }

    public string AgendaItemId { get; set; }

    public AddEditMinutesItemViewModel Clone() 
    {
        return new AddEditMinutesItemViewModel 
        {
            Order = Order,
            Type = Type,
            Title = Title,
            Description = Description,
            AgendaId = AgendaId,
            AgendaItemId = AgendaItemId
        };
    }
}