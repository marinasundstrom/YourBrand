using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Tenancy;

public sealed class TenantDatabasePerTenantOptions
{
    private readonly Dictionary<Type, TenantDatabasePerTenantRule> _rules = new();

    internal void Configure(Type contextType, Action<TenantDatabasePerTenantRule> configure)
    {
        ArgumentNullException.ThrowIfNull(contextType);
        ArgumentNullException.ThrowIfNull(configure);

        if (!_rules.TryGetValue(contextType, out var rule))
        {
            rule = new TenantDatabasePerTenantRule();
            _rules[contextType] = rule;
        }

        configure(rule);
    }

    internal void Configure<TContext>(Action<TenantDatabasePerTenantRule> configure)
        where TContext : DbContext
        => Configure(typeof(TContext), configure);

    internal bool TryGetRule(Type contextType, [NotNullWhen(true)] out TenantDatabasePerTenantRule? rule)
        => _rules.TryGetValue(contextType, out rule);

    internal bool TryGetRule<TContext>([NotNullWhen(true)] out TenantDatabasePerTenantRule? rule)
        where TContext : DbContext
        => TryGetRule(typeof(TContext), out rule);
}

public sealed class TenantDatabasePerTenantRule
{
    public Func<TenantId, string, string>? ConnectionStringFactory { get; set; }
}
