
using Skynet.Application.Common.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace Skynet.WebApi.Hubs;

public class WorkerHub : Hub<IWorkerClient>
{

}