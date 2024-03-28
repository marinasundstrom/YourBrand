
using YourBrand.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace YourBrand.WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{

}