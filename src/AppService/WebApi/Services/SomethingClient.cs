
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.WebApi.Hubs;

namespace YourBrand.WebApi.Services;

public class SomethingClient : ISomethingClient
{
    private readonly IHubContext<SomethingHub, ISomethingClient> _somethingHubContext;

    public SomethingClient(IHubContext<SomethingHub, ISomethingClient> somethingHubContext)
    {
        _somethingHubContext = somethingHubContext;
    }

    public async Task ResponseReceived(string message)
    {
        await _somethingHubContext.Clients.All.ResponseReceived(message);
    }
}