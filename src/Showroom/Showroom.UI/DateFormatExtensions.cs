using System;
using System.Collections.Generic;

using System.Globalization;

using Humanizer;
using Humanizer.Localisation;

namespace YourBrand.Showroom;

public static class DateFormatExtensions 
{
    public static string Humanize(this DateTime startDate, DateTime? endDate)
    {
        return $"{startDate.ToString("MMM yyyy")} - {(endDate == null ? "Present" : endDate.GetValueOrDefault().ToString("MMM yyyy"))}"; 
    }

    public static string Humanize2(this DateTime startDate, DateTime? endDate)
    {
        var now = ExperienceExtensions.GetNowDate();
        return $"{((endDate == null ? now : endDate.GetValueOrDefault()).AddMonths(1) - startDate).Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Year, minUnit: Humanizer.Localisation.TimeUnit.Month, precision: 2)}";
    }
}