using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public record QueryOrdersByCustomFieldValueQuery(string CustomFieldId, string? Value) : IRequest<QueryOrdersByCustomFieldValueQueryResponse>
{
    public class QueryOrdersByCustomFieldValueQueryHandler : IRequestHandler<QueryOrdersByCustomFieldValueQuery, QueryOrdersByCustomFieldValueQueryResponse>
    {
        private readonly ILogger<QueryOrdersByCustomFieldValueQueryHandler> _logger;
        private readonly OrdersContext context;

        public QueryOrdersByCustomFieldValueQueryHandler(
            ILogger<QueryOrdersByCustomFieldValueQueryHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<QueryOrdersByCustomFieldValueQueryResponse> Handle(QueryOrdersByCustomFieldValueQuery request, CancellationToken cancellationToken)
        {
            var message = request;

            var orders = await context.Orders
                .IncludeAll()
                .Where(c => c.CustomFields.Any(m => m.CustomFieldId == message.CustomFieldId && m.Value == message.Value))
                .AsNoTracking()
                .ToArrayAsync();

            return new QueryOrdersByCustomFieldValueQueryResponse
            {
                Orders = orders.Select(Mappings.CreateOrderDto)
            };
        }
    }
}

public class QueryOrdersByCustomFieldValueQueryResponse
{
    public IEnumerable<OrderDto> Orders { get; set; } = null!;
}