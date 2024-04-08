using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class UserRepository : IUserRepository
{
    private readonly TimeReportContext _context;

    public UserRepository(TimeReportContext context)
    {
        _context = context;
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
    }

    public async Task<User?> GetUser(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void RemoveUser(User user)
    {
        _context.Users.Remove(user);
    }
}