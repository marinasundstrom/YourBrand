using YourBrand.Identity;
using YourBrand.Invoicing.Client;

namespace YourBrand.Sales;

public static class ServiceExtensions2
{
    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInvoicingClients((sp, http) =>
        {
            http.BaseAddress = new Uri($"https://localhost:5174/api/invoicing/");
        }, b => b.AddHttpMessageHandler<AuthForwardHandler>());

        return services;
    }
}
