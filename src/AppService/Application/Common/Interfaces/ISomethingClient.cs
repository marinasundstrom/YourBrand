namespace YourBrand.Application.Common.Interfaces;

public interface ISomethingClient
{
    Task ResponseReceived(string message);
}