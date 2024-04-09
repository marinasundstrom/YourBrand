using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class UserRepository(MessengerContext context) : IUserRepository
{
    public async Task<User?> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}