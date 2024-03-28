namespace YourBrand.Sales.API.Features.OrderManagement.Domain;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
