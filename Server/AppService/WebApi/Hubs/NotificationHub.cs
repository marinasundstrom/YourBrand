
using Skynet.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Skynet.WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{

}