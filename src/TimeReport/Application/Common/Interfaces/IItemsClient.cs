using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Common.Interfaces;

public interface IItemsClient
{
    System.Threading.Tasks.Task ItemAdded(User item);

    System.Threading.Tasks.Task ItemDeleted(string id, string name);

    System.Threading.Tasks.Task ImageUploaded(string id, string image);
}