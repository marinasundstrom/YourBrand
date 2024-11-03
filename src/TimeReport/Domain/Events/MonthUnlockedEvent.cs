﻿
using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record MonthUnlocked : DomainEvent
{
    public MonthUnlocked(int year, int month)
    {
        Year = year;
        Month = month;
    }

    public int Year { get; }

    public int Month { get; }
}