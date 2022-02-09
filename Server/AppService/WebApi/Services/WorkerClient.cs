
using Catalog.Application.Common.Interfaces;
using Catalog.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace Catalog.WebApi.Services;

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