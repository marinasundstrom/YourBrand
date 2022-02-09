using System.Security.Claims;

using Catalog.WebApi;

using Microsoft.AspNetCore.SignalR;

public class EmailBasedUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
    }
}