using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Application.OrderStatuses
{
    public class GetOrderStatusesQueryResponse
    {
        public IEnumerable<OrderStatusDto> OrderStatuses { get; set; } = null!;

        public int Total { get; set; }
    }
}

