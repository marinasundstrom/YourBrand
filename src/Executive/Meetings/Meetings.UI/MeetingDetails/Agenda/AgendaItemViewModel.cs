using System;
using System.ComponentModel.DataAnnotations;

namespace YourBrand.Meetings.MeetingDetails.Agenda;

public class AgendaItemViewModel
{
    public string Id { get; set; }

    public int Order { get; set; }

    [Required]
    public string Title { get; set; }

    public AgendaItemType Type { get; set; }

    public AgendaItemState State { get; set; }

    public AgendaItemPhase Phase { get; set; }

    public string Description { get; set; }

    public TimeSpan? EstimatedStartTime { get; set; }

    public TimeSpan? EstimatedEndTime { get; set; }

    public TimeSpan? EstimatedDuration { get; set; }

    public Motion Motion { get; set; }

    public List<AgendaItemViewModel> SubItems { get; set; }

    public AgendaItemViewModel Clone()
    {
        return new AgendaItemViewModel
        {
            Id = Id,
            Order = Order,
            Title = Title,
            Type = Type,
            State = State,
            Phase = Phase,
            Description = Description,
            EstimatedStartTime = EstimatedStartTime,
            EstimatedEndTime = EstimatedEndTime,
            EstimatedDuration = EstimatedDuration,
            Motion = Motion,
            SubItems = SubItems?.Select(x => x.Clone()).ToList() ?? new List<AgendaItemViewModel>()
        };
    }
}