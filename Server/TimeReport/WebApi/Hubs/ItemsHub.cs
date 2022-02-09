using System;

using Microsoft.AspNetCore.SignalR;

using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Hubs;

public class ItemsHub : Hub<IItemsClient>
{

}