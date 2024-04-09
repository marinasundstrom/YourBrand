
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.WebApi.Hubs;

namespace YourBrand.WebApi.Services;

public class WorkerClient(IHubContext<WorkerHub, IWorkerClient> workerHubContext) : IWorkerClient
{
    public async Task NotificationReceived(string message)
    {
        await workerHubContext.Clients.All.NotificationReceived(message);
    }
}