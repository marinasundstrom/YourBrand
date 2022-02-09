
using Catalog.Application.Common.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace Catalog.WebApi.Hubs;

public class WorkerHub : Hub<IWorkerClient>
{

}