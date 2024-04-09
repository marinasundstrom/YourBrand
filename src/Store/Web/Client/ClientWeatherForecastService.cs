using System.Net.Http.Json;

using BlazorApp;

namespace Client;

public sealed class ClientWeatherForecastService(HttpClient httpClient) : IWeatherForecastService
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(DateOnly startDate, CancellationToken cancellationToken = default)
    {
        var startDateStr = startDate.ToString("o");

        return await httpClient.GetFromJsonAsync<WeatherForecast[]>($"/api/weatherforecast?startDate={startDateStr}");
    }
}