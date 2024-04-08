
using YourBrand.Identity;
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class ConversationParticipant : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Conversation Conversation { get; set; } = null!;

    public User User { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}