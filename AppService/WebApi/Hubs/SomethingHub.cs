
using YourBrand.Application.Common.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace YourBrand.WebApi.Hubs;

public class SomethingHub : Hub<ISomethingClient>
{

}