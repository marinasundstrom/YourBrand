using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.Specifications;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : notnull
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(ISpecification<T> specification);
    Task<T?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);
    void Add(T entity);
    void Remove(T entity);
}