using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}