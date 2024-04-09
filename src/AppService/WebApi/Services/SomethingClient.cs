
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.WebApi.Hubs;

namespace YourBrand.WebApi.Services;

public class SomethingClient(IHubContext<SomethingHub, ISomethingClient> somethingHubContext) : ISomethingClient
{
    public async Task ResponseReceived(string message)
    {
        await somethingHubContext.Clients.All.ResponseReceived(message);
    }
}