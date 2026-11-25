using YourBrand.Identity;
using YourBrand.Invoicing.Client;
using YourBrand.TimeReport.Application.Projects.Expenses.Queries;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(GetExpensesQuery)));

        return services;
    }
    
    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInvoicingClients((sp, http) =>
        {
            http.BaseAddress = new Uri($"https://localhost:5174/api/invoicing/");
        }, b => b.AddHttpMessageHandler<AuthForwardHandler>());

        return services;
    }
}
