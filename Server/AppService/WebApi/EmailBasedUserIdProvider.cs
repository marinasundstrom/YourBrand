using System.Security.Claims;

using YourBrand.WebApi;

using Microsoft.AspNetCore.SignalR;

public class EmailBasedUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}