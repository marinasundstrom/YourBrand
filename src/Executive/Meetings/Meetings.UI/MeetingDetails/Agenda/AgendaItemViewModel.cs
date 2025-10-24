using System;
using System.Collections.Generic;
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

    public string Description { get; set; }

    public TimeSpan? EstimatedStartTime { get; set; }

    public TimeSpan? EstimatedEndTime { get; set; }

    public TimeSpan? EstimatedDuration { get; set; }

    public Motion Motion { get; set; }

    public List<AgendaItemViewModel> SubItems { get; set; }

    public List<AgendaItemValidationViewModel> Validations { get; set; } = new();

    public AgendaItemViewModel Clone()
    {
        return new AgendaItemViewModel
        {
            Id = Id,
            Order = Order,
            Title = Title,
            Type = Type,
            State = State,
            Description = Description,
            EstimatedStartTime = EstimatedStartTime,
            EstimatedEndTime = EstimatedEndTime,
            EstimatedDuration = EstimatedDuration,
            Motion = Motion,
            SubItems = SubItems?.Select(x => x.Clone()).ToList() ?? new List<AgendaItemViewModel>(),
            Validations = Validations?.Select(x => x.Clone()).ToList() ?? new List<AgendaItemValidationViewModel>()
        };
    }
}

public class AgendaItemValidationViewModel
{
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsBlocking { get; set; }

    public AgendaItemValidationViewModel Clone() => new()
    {
        Code = Code,
        Message = Message,
        IsBlocking = IsBlocking
    };
}