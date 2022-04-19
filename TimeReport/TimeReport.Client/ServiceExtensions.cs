using Microsoft.Extensions.DependencyInjection;

using YourBrand.TimeReport.Client;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddTimeReportClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        builder(
            services.AddHttpClient(nameof(ITimeSheetsClient), configureClient)
            .AddTypedClient<ITimeSheetsClient>((http, sp) => new TimeSheetsClient(http)));

        builder(
            services.AddHttpClient(nameof(IProjectsClient), configureClient)
            .AddTypedClient<IProjectsClient>((http, sp) => new ProjectsClient(http)));

        builder(
            services.AddHttpClient(nameof(IActivitiesClient), configureClient)
            .AddTypedClient<IActivitiesClient>((http, sp) => new ActivitiesClient(http)));

        builder(
            services.AddHttpClient(nameof(IActivityTypesClient), configureClient)
            .AddTypedClient<IActivityTypesClient>((http, sp) => new ActivityTypesClient(http)));

        builder(
            services.AddHttpClient(nameof(IReportsClient), configureClient)
            .AddTypedClient<IReportsClient>((http, sp) => new ReportsClient(http)));

        builder(
            services.AddHttpClient(nameof(IUsersClient), configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http)));
            
        builder(
            services.AddHttpClient(nameof(IExpensesClient), configureClient)
            .AddTypedClient<IExpensesClient>((http, sp) => new ExpensesClient(http)));

        builder(
            services.AddHttpClient(nameof(IExpenseTypesClient), configureClient)
            .AddTypedClient<IExpenseTypesClient>((http, sp) => new ExpenseTypesClient(http)));

        return services;
    }
}