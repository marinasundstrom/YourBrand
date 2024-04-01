using Microsoft.AspNetCore.SignalR;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Hubs;

public class ItemsHub : Hub<IItemsClient>
{

}