
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.WebApi.Hubs;

public class WorkerHub : Hub<IWorkerClient>
{

}