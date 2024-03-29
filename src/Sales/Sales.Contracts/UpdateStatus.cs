namespace YourBrand.Sales.Contracts;

public record UpdateStatus(string OrderId, OrderStatus Status);

public record ProductPriceChanged(string ProductId, decimal OldPrice, decimal NewPrice);