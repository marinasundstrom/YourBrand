using System;

using Microsoft.AspNetCore.SignalR;

using YourCompany.TimeReport.Application.Common.Interfaces;

namespace YourCompany.TimeReport.Hubs;

public class ItemsHub : Hub<IItemsClient>
{

}