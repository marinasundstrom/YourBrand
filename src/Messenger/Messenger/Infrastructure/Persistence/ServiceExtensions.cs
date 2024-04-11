
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Scrutor;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Repositories;
using YourBrand.Messenger.Infrastructure.Idempotence;
using YourBrand.Messenger.Infrastructure.Persistence.Interceptors;
using YourBrand.Messenger.Infrastructure.Persistence.Repositories;
using YourBrand.Messenger.Infrastructure.Services;

namespace YourBrand.Messenger.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MessengerContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IMessengerContext>(sp => sp.GetRequiredService<MessengerContext>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

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