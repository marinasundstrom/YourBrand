using Microsoft.AspNetCore.SignalR;

namespace YourBrand.Transactions.Hubs;

public class TransactionsHub : Hub<ITransactionsHubClient>
{

}
