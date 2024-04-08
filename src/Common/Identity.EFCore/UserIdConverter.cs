using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace YourBrand.Identity;

public sealed class UserIdConverter : ValueConverter<UserId, string>
{
    public UserIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}