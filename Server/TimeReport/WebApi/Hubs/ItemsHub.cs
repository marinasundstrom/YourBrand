using System;

using Microsoft.AspNetCore.SignalR;

using Skynet.TimeReport.Application.Common.Interfaces;

namespace Skynet.TimeReport.Hubs;

public class ItemsHub : Hub<IItemsClient>
{

}