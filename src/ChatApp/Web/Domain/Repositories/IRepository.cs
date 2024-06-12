using ChatApp.Domain.Specifications;
namespace ChatApp.Domain.Repositories;

public interface IRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : notnull
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(ISpecification<T> specification);
    Task<T?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);
    void Add(T item);
    void Remove(T item);
}
