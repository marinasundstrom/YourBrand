namespace YourBrand.Sales.Features.Services;

public interface IEmailService
{
    Task SendEmail(string recipient, string subject, string body);
}