using TimeReport.Domain.Entities;

namespace TimeReport.Application.Common.Interfaces;

public interface IItemsClient
{
    Task ItemAdded(User item);

    Task ItemDeleted(string id, string name);

    Task ImageUploaded(string id, string image);
}