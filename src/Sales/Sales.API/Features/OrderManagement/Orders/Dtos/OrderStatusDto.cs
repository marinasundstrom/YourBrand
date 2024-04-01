namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

public record OrderStatusDto
(
    int Id,
    string Name,
    string Handle,
    string? Description
);