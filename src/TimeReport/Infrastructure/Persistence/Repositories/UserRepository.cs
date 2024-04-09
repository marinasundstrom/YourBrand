using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class UserRepository(TimeReportContext context) : IUserRepository
{
    public void AddUser(User user)
    {
        context.Users.Add(user);
    }

    public async Task<User?> GetUser(UserId id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void RemoveUser(User user)
    {
        context.Users.Remove(user);
    }
}