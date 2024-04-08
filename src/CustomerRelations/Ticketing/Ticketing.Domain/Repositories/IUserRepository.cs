using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Domain.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{

}