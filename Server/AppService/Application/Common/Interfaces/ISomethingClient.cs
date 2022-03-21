namespace YourCompany.Application.Common.Interfaces;

public interface ISomethingClient
{
    Task ResponseReceived(string message);
}