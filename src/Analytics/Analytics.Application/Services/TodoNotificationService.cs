using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Analytics.Application.Hubs;

public class TodoNotificationService(IHubContext<TodosHub, ITodosHubClient> hubsContext) : ITodoNotificationService
{
}