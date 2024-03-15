namespace BlazorApp;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(DateOnly startDate, CancellationToken cancellationToken = default);
}