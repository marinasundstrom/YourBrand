
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{

}