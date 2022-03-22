
using YourBrand.Application;
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Items;
using YourBrand.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace YourBrand.WebApi.Services;

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