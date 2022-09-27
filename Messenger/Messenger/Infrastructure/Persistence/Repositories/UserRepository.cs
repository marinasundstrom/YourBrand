using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class UserRepository : IUserRepository
{
    private readonly MessengerContext _context;

    public UserRepository(MessengerContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
