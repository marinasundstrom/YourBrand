
using Catalog.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Catalog.WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<INotificationClient>
{

}