using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data;

public sealed class WeatherForecastService : IWeatherForecastService
{
    static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(DateOnly startDate, CancellationToken cancellationToken = default)
    {
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        return Task.FromResult<IEnumerable<WeatherForecast>>(forecasts);
    }
}