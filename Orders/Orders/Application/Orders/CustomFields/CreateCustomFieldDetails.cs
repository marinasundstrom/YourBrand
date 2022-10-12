using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;
using Microsoft.Extensions.Logging;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;

namespace YourBrand.Orders.Application.Orders;

public class CreateCustomFieldDetails
{
    public string CustomFieldId { get; set; } = null!;

    public string Value { get; set; } = null!;
}
