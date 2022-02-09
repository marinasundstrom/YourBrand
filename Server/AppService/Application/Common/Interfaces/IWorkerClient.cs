namespace Catalog.Application.Common.Interfaces;

public interface IWorkerClient
{
    Task NotificationReceived(string message);
}