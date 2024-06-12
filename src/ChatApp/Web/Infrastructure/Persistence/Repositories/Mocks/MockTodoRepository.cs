using ChatApp.Domain.Specifications;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.Repositories.Mocks;

public sealed class MockTodoRepository : IMessageRepository
{
    private readonly MockUnitOfWork mockUnitOfWork;

    public MockTodoRepository(MockUnitOfWork mockUnitOfWork)
    {
        this.mockUnitOfWork = mockUnitOfWork;
    }

    public void Add(Message item)
    {
        mockUnitOfWork.Items.Add(item);
        mockUnitOfWork.NewItems.Add(item);
    }

    public void Dispose()
    {
        foreach (var item in mockUnitOfWork.NewItems)
        {
            mockUnitOfWork.Items.Remove(item);
        }
    }

    public Task<Message?> FindByIdAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        var item = mockUnitOfWork.Items
            .OfType<Message>()
            .FirstOrDefault(x => x.Id.Equals(id));

        return Task.FromResult(item);
    }

    public IQueryable<Message> GetAll()
    {
        return mockUnitOfWork.Items
            .OfType<Message>()
            .AsQueryable();
    }

    public IQueryable<Message> GetAll(ISpecification<Message> specification)
    {
        return mockUnitOfWork.Items
            .OfType<Message>()
            .AsQueryable()
            .Where(specification.Criteria);
    }

    public void Remove(Message item)
    {
        mockUnitOfWork.Items.Remove(item);
    }
}

