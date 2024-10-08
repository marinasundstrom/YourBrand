using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Identity;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
}