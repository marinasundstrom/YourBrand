namespace YourBrand.Analytics.Infrastructure.Services;

sealed class DateTimeService : IDateTime
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}

