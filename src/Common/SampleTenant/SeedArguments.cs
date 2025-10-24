using System.Linq;

namespace YourBrand;

public static class SeedArguments
{
    public static bool TryGetTenantId(string[] args, out string tenantId)
    {
        tenantId = string.Empty;

        if (args is null || args.Length == 0)
        {
            return false;
        }

        if (!args.Contains("--seed"))
        {
            return false;
        }

        string? parsed = Parse(args);

        if (string.IsNullOrWhiteSpace(parsed))
        {
            tenantId = string.Empty;
            return false;
        }

        tenantId = parsed;
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

        return Parse(args);
    }

    private static string? Parse(string[] args)
    {
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
}
