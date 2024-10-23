using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}