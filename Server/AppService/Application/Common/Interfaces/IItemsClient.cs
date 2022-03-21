using YourCompany.Application.Items;

namespace YourCompany.Application.Common.Interfaces;

public interface IItemsClient
{
    Task ItemAdded(ItemDto item);

    Task ItemDeleted(string id, string name);

    Task ImageUploaded(string id, string image);
}