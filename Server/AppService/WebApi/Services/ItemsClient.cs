
using YourCompany.Application;
using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Items;
using YourCompany.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace YourCompany.WebApi.Services;

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