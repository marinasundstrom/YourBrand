
using Skynet.Application.Common.Interfaces;
using Skynet.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace Skynet.WebApi.Services;

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