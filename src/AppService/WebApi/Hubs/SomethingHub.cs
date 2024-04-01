
using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.WebApi.Hubs;

public class SomethingHub : Hub<ISomethingClient>
{

}