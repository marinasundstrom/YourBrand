using Microsoft.AspNetCore.SignalR;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Hubs;

public class ItemsHub : Hub<IItemsClient>
{

}