using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.MeetingDetails.Agenda;

public class AgendaItemViewModel
{
    public string Id { get; set; }

    public int Order { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public Motion Motion { get; set; }

    public AgendaItemViewModel Clone()
    {
        return new AgendaItemViewModel
        {
            Id = Id,
            Order = Order,
            Title = Title,
            Description = Description,
            Motion = Motion
        };
    }
}