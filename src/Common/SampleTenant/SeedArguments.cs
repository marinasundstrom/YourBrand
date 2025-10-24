using System;
using System.Linq;

namespace YourBrand;

public static class SeedArguments
{
    public static bool TryGetTenantId(string[] args, out string tenantId)
    {
        tenantId = TenantConstants.TenantId;

        if (args is null || args.Length == 0)
        {
            tenantId = string.Empty;
            return false;
        }

        if (!args.Contains("--seed"))
        {
            tenantId = string.Empty;
            return false;
        }

        string? parsed = Parse(args);

        tenantId = string.IsNullOrWhiteSpace(parsed)
            ? TenantConstants.TenantId
            : parsed;

        return true;
    }

    public static string? GetTenantId(string[] args)
    {
        if (args is null || args.Length == 0)
        {
            return null;
        }

        if (!args.Contains("--seed"))
        {
            return null;
        }

        string? parsed = Parse(args);

        return string.IsNullOrWhiteSpace(parsed)
            ? TenantConstants.TenantId
            : parsed;
    }

    private static string? Parse(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--tenantId")
            {
                if (i + 1 < args.Length && !IsOption(args[i + 1]))
                {
                    return args[i + 1];
                }

                return null;
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--")
            {
                return i + 1 < args.Length ? args[i + 1] : null;
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed")
            {
                if (i + 1 < args.Length && args[i + 1] != "--")
                {
                    return args[i + 1];
                }

                if (i + 2 < args.Length && args[i + 1] == "--")
                {
                    return args[i + 2];
                }
            }
        }

        return null;
    }

    private static bool IsOption(string value) => value.StartsWith("--", StringComparison.Ordinal);
}
