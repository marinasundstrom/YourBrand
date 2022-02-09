
using Skynet.Application;
using Skynet.Application.Common.Interfaces;
using Skynet.Application.Items;
using Skynet.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace Skynet.WebApi.Services;

public class ItemsClient : IItemsClient
{
    private readonly IHubContext<ItemsHub, IItemsClient> _itemsHubContext;

    public ItemsClient(IHubContext<ItemsHub, IItemsClient> itemsHubContext)
    {
        _itemsHubContext = itemsHubContext;
    }

    public async Task ImageUploaded(string id, string image)
    {
        await _itemsHubContext.Clients.All.ImageUploaded(id, image);
    }

    public async Task ItemAdded(ItemDto item)
    {
        await _itemsHubContext.Clients.All.ItemAdded(item);
    }

    public async Task ItemDeleted(string id, string name)
    {
        await _itemsHubContext.Clients.All.ItemDeleted(id, name);
    }
}