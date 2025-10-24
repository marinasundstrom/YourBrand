using System;
using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YourBrand.Tenancy;

public sealed class TenantDatabasePerTenantBuilder(IServiceCollection services)
{
    private readonly IServiceCollection _services = services;

    public TenantDatabasePerTenantRuleBuilder ForContext<TContext>()
        where TContext : DbContext
    {
        _services.TryAddScoped<TenantDbConnectionInterceptor<TContext>>();

        return new TenantDatabasePerTenantRuleBuilder(_services, typeof(TContext));
    }
}

public sealed class TenantDatabasePerTenantRuleBuilder(IServiceCollection services, Type contextType)
{
    private readonly IServiceCollection _services = services;
    private readonly Type _contextType = contextType;

    public TenantDatabasePerTenantRuleBuilder UseConnectionString(Func<TenantId, string, string> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _services.Configure<TenantDatabasePerTenantOptions>(options =>
        {
            options.Configure(_contextType, rule => rule.ConnectionStringFactory = factory);
        });

        return this;
    }

    public TenantDatabasePerTenantRuleBuilder WithDatabase(Func<TenantId, string> databaseFactory)
    {
        ArgumentNullException.ThrowIfNull(databaseFactory);

        return UseConnectionString((tenantId, connectionString) =>
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var databaseName = databaseFactory(tenantId);

            if (builder.ContainsKey("Database"))
            {
                builder["Database"] = databaseName;
            }
            else if (builder.ContainsKey("Initial Catalog"))
            {
                builder["Initial Catalog"] = databaseName;
            }
            else
            {
                builder["Database"] = databaseName;
            }

            return builder.ConnectionString;
        });
    }

    public TenantDatabasePerTenantRuleBuilder WithTemplate(string template, string placeholder = "{tenantId}")
    {
        ArgumentNullException.ThrowIfNull(template);

        if (string.IsNullOrEmpty(placeholder))
        {
            throw new ArgumentException("Placeholder must be provided.", nameof(placeholder));
        }

        return UseConnectionString((tenantId, _) => template.Replace(placeholder, tenantId.Value, StringComparison.OrdinalIgnoreCase));
    }
}
