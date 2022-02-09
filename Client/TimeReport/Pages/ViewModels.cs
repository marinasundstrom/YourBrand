using System;
using System.ComponentModel.DataAnnotations;

using TimeReport.Client;

namespace TimeReport.Pages;

public class ActivityModel
{
    public ActivityDto? Activity { get; set; }

    [ValidateComplexType]
    public List<EntryModel> Entries { get; set; } = new List<EntryModel>();

    public double TotalHours => Entries.Sum(e => e.Hours.GetValueOrDefault());
}

public class EntryModel
{
    public string? Id { get; set; }

    public DateTime Date { get; set; }

    [Range(0, 8)]
    public double? Hours { get; set; }

    public string? Description { get; set; }

    public EntryStatusDto Status { get; set; }
}