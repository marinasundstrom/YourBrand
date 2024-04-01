using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using Scrutor;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Infrastructure.BackgroundJobs;
using YourBrand.Showroom.Infrastructure.Idempotence;
using YourBrand.Showroom.Infrastructure.Persistence;
using YourBrand.Showroom.Infrastructure.Persistence.Interceptors;
using YourBrand.Showroom.Infrastructure.Services;

namespace YourBrand.Showroom.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

                configure
                    .AddJob<ProcessOutboxMessagesJob>(jobKey)
                    .AddTrigger(trigger => trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(10)
                            .RepeatForever()));

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

        services.AddQuartzHostedService();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<ShowroomContext>(
            configuration.GetConnectionString("mssql", "Showroom") ?? configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure());

        services.AddScoped<IShowroomContext>(sp => sp.GetRequiredService<ShowroomContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch (DecorationException exc) //when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}