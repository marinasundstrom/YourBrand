using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp;

[AttributeUsage(AttributeTargets.Method)]
internal class HubServerProxyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
internal class HubClientProxyAttribute : Attribute { }

public static partial class HubConnectionExtensions
{
    [HubClientProxy]
    public static partial IDisposable ClientRegistration<T>(this HubConnection connection, T provider);

    [HubServerProxy]
    public static partial T ServerProxy<T>(this HubConnection connection);
}