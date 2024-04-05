namespace YourBrand.Portal;

public interface IUserSearchProvider
{
    Task<IEnumerable<User>> QueryUsersAsync(string? searchTerm, CancellationToken cancellationToken = default);
}

public record User(string Id, string Name)
{
    public string GetDisplayName() => Name;
}