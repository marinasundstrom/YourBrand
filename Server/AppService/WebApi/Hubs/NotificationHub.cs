
using YourCompany.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace YourCompany.WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{

}