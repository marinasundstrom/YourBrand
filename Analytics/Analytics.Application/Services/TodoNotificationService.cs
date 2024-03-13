using Microsoft.AspNetCore.SignalR;
using YourBrand.Analytics.Application.Services;

namespace YourBrand.Analytics.Application.Hubs;

public class TodoNotificationService : ITodoNotificationService
{
    private readonly IHubContext<TodosHub, ITodosHubClient> hubsContext;

    public TodoNotificationService(IHubContext<TodosHub, ITodosHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }


}