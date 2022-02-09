using System;

using Catalog.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Catalog.WebApi.Hubs;

[Authorize]
public class ItemsHub : Hub<IItemsClient>
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}