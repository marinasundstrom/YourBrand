using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YourBrand.Tenancy;

public sealed class TenantDbConnectionInterceptor<TContext>(
    ITenantContext tenantContext,
    IOptions<TenantDatabasePerTenantOptions> options,
    ILogger<TenantDbConnectionInterceptor<TContext>> logger) : DbConnectionInterceptor
    where TContext : DbContext
{
    private readonly ITenantContext _tenantContext = tenantContext;
    private readonly IOptions<TenantDatabasePerTenantOptions> _options = options;
    private readonly ILogger _logger = logger;

    private TenantDatabasePerTenantRule? _rule;

    public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        ApplyTenantConnectionString(connection, eventData.Context);

        return base.ConnectionOpening(connection, eventData, result);
    }

    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
    {
        ApplyTenantConnectionString(connection, eventData.Context);

        return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }

    private void ApplyTenantConnectionString(DbConnection connection, DbContext? dbContext)
    {
        if (dbContext is not TContext)
        {
            return;
        }

        var tenantId = _tenantContext.TenantId;

        if (tenantId is null)
        {
            return;
        }

        var rule = _rule ??= _options.Value.TryGetRule(typeof(TContext), out var configuredRule)
            ? configuredRule
            : null;

        if (rule?.ConnectionStringFactory is null)
        {
            return;
        }

        var currentConnectionString = connection.ConnectionString;

        if (string.IsNullOrWhiteSpace(currentConnectionString))
        {
            return;
        }

        var resolvedConnectionString = rule.ConnectionStringFactory(tenantId.Value, currentConnectionString);

        if (string.IsNullOrWhiteSpace(resolvedConnectionString) || string.Equals(currentConnectionString, resolvedConnectionString, StringComparison.Ordinal))
        {
            return;
        }

        connection.ConnectionString = resolvedConnectionString;

        _logger.LogDebug("Applied tenant specific connection string for context {Context} and tenant {TenantId}.", typeof(TContext).Name, tenantId.Value.Value);
    }
}
