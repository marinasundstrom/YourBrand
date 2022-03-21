using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Application.Common.Interfaces;

public interface IItemsClient
{
    Task ItemAdded(User item);

    Task ItemDeleted(string id, string name);

    Task ImageUploaded(string id, string image);
}