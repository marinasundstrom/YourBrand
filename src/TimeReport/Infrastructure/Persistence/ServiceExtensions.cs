using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Scrutor;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Idempotence;
using YourBrand.TimeReport.Infrastructure.Persistence.Interceptors;
using YourBrand.TimeReport.Infrastructure.Services;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<TimeReportContext>(
            configuration.GetConnectionString("mssql", "TimeReport") ?? configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure());

        services.AddScoped<ITimeReportContext>(sp => sp.GetRequiredService<TimeReportContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITimeSheetRepository, TimeSheetRepository>();
        services.AddScoped<IReportingPeriodRepository, ReportingPeriodRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch (DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}