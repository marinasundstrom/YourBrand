
using YourCompany.Application.Common.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace YourCompany.WebApi.Hubs;

public class WorkerHub : Hub<IWorkerClient>
{

}