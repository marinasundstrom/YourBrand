using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public static class ExperienceExtensions
{
    public static DateTime GetNowDate()
    {
        return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
    }

    public static DateTime GetStartDate(this Experience experience)
    {
        return new DateTime(experience.StartDate.Year, experience.StartDate.Month, 1);
    }

    public static DateTime? GetEndDate(this Experience experience)
    {
        DateTime? endDate = null;

        if (experience.EndDate != null)
        {
            var _endDate = experience.EndDate.GetValueOrDefault();
            endDate = new DateTime(_endDate.Year, _endDate.Month, DateTime.DaysInMonth(_endDate.Year, _endDate.Month));
        }

        return endDate;
    }
}