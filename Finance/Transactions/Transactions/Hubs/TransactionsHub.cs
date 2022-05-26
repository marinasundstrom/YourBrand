using Microsoft.AspNetCore.SignalR;

namespace Transactions.Hubs;

public class TransactionsHub : Hub<ITransactionsHubClient>
{

}
