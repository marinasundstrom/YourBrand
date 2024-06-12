using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public interface ISoftDelete
{
    UserId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}
