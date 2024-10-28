namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

public record OrderTypeDto
(
    int Id,
    string Name,
    string Handle,
    string? Description
);