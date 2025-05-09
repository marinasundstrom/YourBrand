﻿
using System.Globalization;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Domain.Entities;

public class TimeSheetTask : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    private readonly HashSet<Entry> _entries = new HashSet<Entry>();

    public TimeSheetTask(TimeSheet timeSheet, Project project, Task task) : base(Guid.NewGuid().ToString())
    {
        TimeSheet = timeSheet;
        Project = project;
        Task = task;

        AddDomainEvent(new TimeSheetTaskAddedEvent(TimeSheet.Id, Id, Task.Id));
    }

    internal TimeSheetTask()
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public TimeSheet TimeSheet { get; private set; } = null!;

    public Project Project { get; private set; } = null!;

    public Task Task { get; private set; } = null!;

    public IReadOnlyCollection<Entry> Entries => _entries;

    // public decimal? HourlyRate { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public Entry AddEntry(DateOnly date, double? hours, string? description)
    {
        int week = GetWeekFromDate(date);

        if (TimeSheet.Year != date.Year || TimeSheet.Week != week)
        {
            throw new InvalidOperationException("Date is not in Week.");
        }

        if (TimeSheet.Entries.Any(e => e.Task.Id == Task.Id && e.Date == date))
        {
            throw new InvalidOperationException("Entry for this date already exists");
        }

        if (hours < 0)
        {
            throw new InvalidOperationException("Hours must not be negative,");
        }

        if (hours > 8)
        {
            throw new InvalidOperationException("Hours must not be greater than 8.");
        }

        var entry = new Entry(TimeSheet.User, Project, Task, TimeSheet, this, date, hours, description);
        entry.OrganizationId = OrganizationId;
        _entries.Add(entry);
        TimeSheet.AddEntry(entry);
        return entry;
    }

    private static int GetWeekFromDate(DateOnly date)
    {
        return ISOWeek.GetWeekOfYear(
            date.ToDateTime(
                TimeOnly.FromTimeSpan(
                    TimeSpan.FromMinutes(10))));
    }

    public Entry? GetEntryByDate(DateOnly date)
    {
        return _entries.FirstOrDefault(e => e.Date == date);
    }
}