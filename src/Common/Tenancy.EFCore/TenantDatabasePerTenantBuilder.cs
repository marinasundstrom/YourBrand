using System;
using System.Data.Common;
using System.Globalization;

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

    public TenantDatabasePerTenantRuleBuilder WithDatabasePrefix(string databaseNamePrefix)
    {
        if (string.IsNullOrWhiteSpace(databaseNamePrefix))
        {
            throw new ArgumentException("Database name prefix must be provided.", nameof(databaseNamePrefix));
        }

        return WithDatabase(tenantId => $"{databaseNamePrefix}_{NormalizeDatabaseNamePart(tenantId.Value)}");
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

    private static string NormalizeDatabaseNamePart(object value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Tenant id cannot be empty when resolving the tenant database name.");
        }

        if (Guid.TryParse(text, out var guid))
        {
            return guid.ToString("N", CultureInfo.InvariantCulture);
        }

        return text.Replace("-", string.Empty, StringComparison.Ordinal);
    }
}
