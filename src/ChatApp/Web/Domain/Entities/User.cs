using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public class User : AggregateRoot<UserId>, IAuditable
{
#nullable disable

    protected User() : base(new UserId(null))
    {
    }

#nullable restore

    public User(UserId id, string name, string email)
        : base(id)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
