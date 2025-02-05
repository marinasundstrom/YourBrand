﻿using System.ComponentModel.DataAnnotations;

using YourBrand.TimeReport.Client;

namespace YourBrand.TimeReport.TimeSheets;

public class TaskModel
{
    public YourBrand.TimeReport.Client.Task? Task { get; set; }

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

    public EntryStatus Status { get; set; }
}