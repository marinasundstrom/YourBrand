using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Infrastructure.Persistence;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Interceptors;
using YourBrand.IdentityManagement.Infrastructure.Services;
using Quartz;
using YourBrand.IdentityManagement.Infrastructure.BackgroundJobs;
using MediatR;
using YourBrand.IdentityManagement.Infrastructure.Idempotence;
using Scrutor;

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
                            .RepeatForever()));

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("mssql", "Users") ?? configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        try 
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch(DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();
        
        return services;
    }
}