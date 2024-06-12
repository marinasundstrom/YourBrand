using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}