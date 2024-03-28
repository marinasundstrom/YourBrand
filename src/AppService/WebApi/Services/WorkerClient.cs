
using YourBrand.Application.Common.Interfaces;
using YourBrand.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace YourBrand.WebApi.Services;

public class WorkerClient : IWorkerClient
{
    private readonly IHubContext<WorkerHub, IWorkerClient> _workerHubContext;

    public WorkerClient(IHubContext<WorkerHub, IWorkerClient> workerHubContext)
    {
        _workerHubContext = workerHubContext;
    }

    public async Task NotificationReceived(string message)
    {
        await _workerHubContext.Clients.All.NotificationReceived(message);
    }
}