using System.Linq.Expressions;
using System.Reflection;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public record CurrencyAmountDto(string Currency, decimal Amount);