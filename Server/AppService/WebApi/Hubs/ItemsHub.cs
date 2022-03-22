using System;

using YourBrand.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace YourBrand.WebApi.Hubs;

[Authorize]
public class ItemsHub : Hub<IItemsClient>
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}