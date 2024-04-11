using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using Scrutor;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Infrastructure.BackgroundJobs;
using YourBrand.IdentityManagement.Infrastructure.Idempotence;
using YourBrand.IdentityManagement.Infrastructure.Persistence;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Interceptors;
using YourBrand.IdentityManagement.Infrastructure.Services;

namespace YourBrand.IdentityManagement.Infrastructure;

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
                            .RepeatForever())); ;
            });

        services.AddQuartzHostedService();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);

            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddDbContext<ApplicationDbContextWithTenantFilter>((sp, options) =>
        {
            options.UseSqlServer(connectionString);

            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });


        services.AddTransient<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContextWithTenantFilter>());

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