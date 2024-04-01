using Blazored.LocalStorage;
using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

using YourBrand.StoreFront;

namespace Client.Analytics;

public sealed class AnalyticsService : IDisposable
{
    private readonly IAnalyticsClient analyticsClient;
    private readonly NavigationManager navigationManager;
    private readonly ILocalStorageService localStorageService;
    private readonly ISessionStorageService sessionStorageService;
    private readonly IServiceProvider serviceProvider;
    string? cid;
    string? sid;
    private string? referrer;
    private IDisposable? locationChangingHandler;


    public AnalyticsService(
        IAnalyticsClient analyticsClient,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService,
        ISessionStorageService sessionStorageService,
        IServiceProvider serviceProvider)
    {
        this.analyticsClient = analyticsClient;
        this.navigationManager = navigationManager;
        this.localStorageService = localStorageService;
        this.sessionStorageService = sessionStorageService;
        this.serviceProvider = serviceProvider;
    }

    public async Task Init()
    {
        var cid = await EnsureClientId();

        await EnsureSessionId(cid);

        locationChangingHandler = navigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    private async Task<string> EnsureClientId()
    {
        cid = await localStorageService.GetItemAsync<string?>("cid");

        if (cid is null)
        {
            cid = await analyticsClient.CreateClientAsync();
            await localStorageService.SetItemAsync("cid", cid);
        }

        return cid;
    }

    private async Task<string> EnsureSessionId(string cid)
    {
        sid = await sessionStorageService.GetItemAsync<string?>("sid");

        if (sid is null)
        {
            try
            {
                sid = await analyticsClient.StartSessionAsync(cid);
                await sessionStorageService.SetItemAsync("sid", sid);
            }
            catch (Exception)
            {
                // Retry

                await localStorageService.RemoveItemAsync("cid");

                await Init();
            }

            GetCoordinate();
        }

        return sid;
    }

    private ValueTask OnLocationChanging(LocationChangingContext arg)
    {
        referrer = navigationManager.Uri;

        return ValueTask.CompletedTask;
    }

    public async Task RegisterEvent(EventData eventData)
    {
        try
        {
            eventData.Data.Add("url", navigationManager.Uri);
            eventData.Data.Add("referrer", GetReferrer());

            sid = await analyticsClient.RegisterEventAsync(cid, sid, eventData);
            if (sid is not null)
            {
                await sessionStorageService.SetItemAsync("sid", sid);

                GetCoordinate();
            }
        }
        catch (ApiException exc) when (exc.StatusCode == 204)
        {
            // This is OK!
        }
    }

    private string GetReferrer()
    {
        using var scope = serviceProvider.CreateScope();

        var jsRuntime = scope.ServiceProvider
            .GetRequiredService<Microsoft.JSInterop.IJSInProcessRuntime>();

        return referrer ?? jsRuntime.Invoke<string>("getReferrer");
    }

    void GetCoordinate()
    {
        using var scope = serviceProvider.CreateScope();

        var geolocationService = scope.ServiceProvider
            .GetRequiredService<Microsoft.JSInterop.IGeolocationService>();

        geolocationService.GetCurrentPosition((args) =>
        {
            analyticsClient.RegisterCoordinatesAsync(cid, sid, new Coordinates()
            {
                Latitude = (float)args.Coords.Latitude,
                Longitude = (float)args.Coords.Longitude
            });

        }, (error) => { });
    }

    public void Dispose()
    {
        locationChangingHandler?.Dispose();
    }
}