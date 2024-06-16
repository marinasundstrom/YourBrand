using Microsoft.AspNetCore.SignalR.Client;

namespace YourBrand.ChatApp;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class HubServerProxyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
internal sealed class HubClientProxyAttribute : Attribute { }

public static partial class HubConnectionExtensions
{
    [HubClientProxy]
    public static partial IDisposable ClientRegistration<T>(this HubConnection connection, T provider);

    [HubServerProxy]
    public static partial T ServerProxy<T>(this HubConnection connection);
}