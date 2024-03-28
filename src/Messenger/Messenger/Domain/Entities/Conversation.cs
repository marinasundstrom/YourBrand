
using YourBrand.Messenger.Domain.Common;
using YourBrand.Messenger.Domain.Events;

namespace YourBrand.Messenger.Domain.Entities;

public class Conversation : AuditableEntity, ISoftDelete
{
    readonly HashSet<ConversationParticipant> _participants = new HashSet<ConversationParticipant>();
    readonly HashSet<Message> _messages = new HashSet<Message>();

    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public IReadOnlyCollection<ConversationParticipant> Participants => _participants;

    public void AddParticipant(User user) => _participants.Add(new ConversationParticipant()
    {
        User = user
    });

    public IReadOnlyCollection<Message> Messages => _messages;

    public void AddMessage(Message message) 
    {
         _messages.Add(message);
        message.AddDomainEvent(new MessagePostedEvent(Id, message.Id));
    }

    public void DeleteMessage(Message message)
    {
        message.Text = String.Empty;
        _messages.Remove(message);
         message.AddDomainEvent(new MessageDeletedEvent(Id, message.Id));
    }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public void RemoveParticipant(ConversationParticipant participant)
    {
        _participants.Add(participant);
    }
}
