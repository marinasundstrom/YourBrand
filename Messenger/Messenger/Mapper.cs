using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName,  user.Email, user.Created, user.Deleted);
    }

    public static Contracts.MessageDto ToDto(this Domain.Entities.Message message)
    {
        return new Contracts.MessageDto(message.Id, message.ConversationId, message.Text, message.ReplyTo?.ToReplyToDto(), message.Receipts?.Select(r => r.ToDto()), message.Replies?.Select(r => r.ToDto()), message.Created, message.CreatedBy!.ToDto(), message.LastModified, message.LastModifiedBy?.ToDto(), message.Deleted, message.DeletedBy?.ToDto());
    }

    public static Contracts.ReplyToMessageDto ToReplyToDto(this Domain.Entities.Message message)
    {
        return new Contracts.ReplyToMessageDto(message.Id, message.ConversationId, message.Text, message.Created, message.CreatedBy!.ToDto(), message.LastModified, message.LastModifiedBy?.ToDto(), message.Deleted, message.DeletedBy?.ToDto());
    }

    public static Contracts.ReceiptDto ToDto(this Domain.Entities.MessageReceipt receipt)
    {
        return new Contracts.ReceiptDto(receipt.Id, receipt.Message.Id, receipt.CreatedBy!.ToDto(), receipt.Created);
    }

    public static Contracts.ConversationDto ToDto(this Domain.Entities.Conversation conversation)
    {
        return new Contracts.ConversationDto(conversation.Id, conversation.Title, conversation.Participants.Select(p => p.ToDto()), conversation.CreatedBy!.ToDto(), conversation.Created);
    }

    public static Contracts.ParticipantDto ToDto(this Domain.Entities.ConversationParticipant participant)
    {
        return new Contracts.ParticipantDto(participant.Id, participant.User.ToDto(), participant.Created);
    }
}