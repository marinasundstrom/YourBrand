using System;
using System.Globalization;

using Shouldly;

using Xunit;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Tests;

public class TimeSheetTest
{
    private Activity _activity = null!;
    private User _user = null!;

    public TimeSheetTest()
    {
        Organization organization = new("id", "Test org", "Descr");

        Project project = new("Test project", "Descr");

        organization.AddProject(project);

        ActivityType activityType = organization.AddActivityType("Test activity type", "Desc");

        _activity = new("Activity", activityType, "Des");

        project.AddActivity(_activity);

        _user = new();
    }

    [Fact(DisplayName = "Can add entry for date in week")]
    public void CanAddEntryForDateInWeek()
    {
        int year = 2022;
        int week = 41;

        TimeSheet timeSheet = new(_user, year, week);

        var timeSheetActivity = timeSheet.AddActivity(_activity);

        var date = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var dateOnly = DateOnly.FromDateTime(date);

        var entry = timeSheetActivity.AddEntry(dateOnly, 7, null);

        timeSheet.Entries.Count.ShouldBe(1);
    }


    [Fact(DisplayName = "Cannot add entry for date not in week")]
    public void CannotAddEntryForDateNotInWeek()
    {
        int year = 2022;
        int week = 41;

        TimeSheet timeSheet = new(_user, year, week);

        var timeSheetActivity = timeSheet.AddActivity(_activity);

        var date = ISOWeek.ToDateTime(year + 1, week + 2, DayOfWeek.Monday);
        var dateOnly = DateOnly.FromDateTime(date);

        Assert.Throws<InvalidOperationException>(() => {
            var entry = timeSheetActivity.AddEntry(dateOnly, 7, null);
        });
    }

    [Fact(DisplayName = "Can add entries for two different dates in week")]
    public void CanAddEntriesForTwoDifferentDatesInWeek()
    {
        int year = 2022;
        int week = 41;

        TimeSheet timeSheet = new(_user, year, week);

        var timeSheetActivity = timeSheet.AddActivity(_activity);

        var date = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var dateOnly = DateOnly.FromDateTime(date);

        var entry = timeSheetActivity.AddEntry(dateOnly, 2, null);

        var date2 = ISOWeek.ToDateTime(year, week, DayOfWeek.Tuesday);
        var dateOnly2 = DateOnly.FromDateTime(date2);

        var entry2 = timeSheetActivity.AddEntry(dateOnly2, 6, null);

        timeSheet.Entries.Count.ShouldBe(2);
    }

    [Fact(DisplayName = "Cannot add entry for a date that has already been added")]
    public void CannotAddEntryForADateThatHasAlreadyBeenAdded()
    {
        int year = 2022;
        int week = 41;

        TimeSheet timeSheet = new(_user, year, week);

        var timeSheetActivity = timeSheet.AddActivity(_activity);

        var date = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var dateOnly = DateOnly.FromDateTime(date);

        var entry = timeSheetActivity.AddEntry(dateOnly, 7, null);

        Assert.Throws<InvalidOperationException>(() => {
            var entry2 = timeSheetActivity.AddEntry(dateOnly, 2, null);
        });
    }
}
