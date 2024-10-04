using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Identity;

namespace YourBrand.Meetings.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}