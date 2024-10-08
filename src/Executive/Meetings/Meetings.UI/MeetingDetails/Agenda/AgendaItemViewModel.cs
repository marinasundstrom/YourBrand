using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.MeetingDetails.Agenda;

public class AgendaItemViewModel
{
    public string Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public AgendaItemViewModel Clone()
    {
        return new AgendaItemViewModel
        {
            Id = Id,
            Title = Title,
            Description = Description
        };
    }
}