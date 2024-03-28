using System.Net.Http.Json;

using BlazorApp;

namespace Client;

public sealed class ClientWeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _httpClient;

    public ClientWeatherForecastService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(DateOnly startDate, CancellationToken cancellationToken = default)
    {
        var startDateStr = startDate.ToString("o");

        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>($"/api/weatherforecast?startDate={startDateStr}");
    }
}