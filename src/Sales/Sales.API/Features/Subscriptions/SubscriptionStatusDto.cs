namespace YourBrand.Sales.Features.SubscriptionManagement;

public record SubscriptionStatusDto
(
    int Id,
    string Name,
    string Handle,
    string? Description
);