namespace YourBrand.Analytics.Domain.Entities;

public class Client : Entity<string>
{
#nullable disable

    protected Client() : base() { }

#nullable restore

    public Client(string id, string userAgent)
    : base(id)
    {
        UserAgent = userAgent;
    }

    public DateTimeOffset? Created { get; private set; } = DateTimeOffset.UtcNow;

    public string UserAgent { get; private set; } = default!;

    public IReadOnlyCollection<Session> Sessions { get; private set; } = new HashSet<Session>();
}
