using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Payments.Hubs;

public class PaymentsHub : Hub<IPaymentsHubClient>
{

}
