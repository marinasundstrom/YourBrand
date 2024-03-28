using System.Reflection;

using Asp.Versioning;

namespace YourBrand.Messenger;

public static class ApiVersions
{
    public static readonly ApiVersion V1 = new ApiVersion(1, 0);

    static IEnumerable<ApiVersion>? _all;

    public static IEnumerable<ApiVersion> All => _all
        ??= typeof(ApiVersions)
        .GetFields(BindingFlags.Static | BindingFlags.Public)
        .Where(x => x.FieldType == typeof(ApiVersion)).Select(x => (ApiVersion)x.GetValue(null)!)
        .ToList().AsEnumerable();
}