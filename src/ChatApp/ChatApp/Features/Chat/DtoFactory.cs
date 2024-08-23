using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

public interface IDtoFactory
{
    ReplyMessageDto CreateReplyMessageDto(Message replyMessage, ChannelParticipant replyMessagePublishedBy, ChannelParticipant? replyMessageEditedBy, ChannelParticipant? replyMessageDeletedBy);
    MessageDto CreateMessageDto(Message message, ChannelParticipant publishedBy, ChannelParticipant? editedBy, ChannelParticipant? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions);
    ParticipantDto CreateParticipantDto(ChannelParticipant participant);
    UserDto CreateUserDto(User user);
    ReactionDto CreateReactionDto(MessageReaction reaction, ChannelParticipant addedBy);
}

public sealed class DtoFactory : IDtoFactory
{
    public ReplyMessageDto CreateReplyMessageDto(Message replyMessage, ChannelParticipant replyMessagePostedBy, ChannelParticipant? replyMessageLastEditedBy, ChannelParticipant? replyMessageDeletedBy)
    {
        return new ReplyMessageDto(
            (Guid)replyMessage.Id,
            replyMessage.ChannelId,
            replyMessage.Content,
            replyMessage.Posted, CreateParticipantDto(replyMessagePostedBy),
            replyMessage.LastEdited, replyMessage.LastEditedById is null ? null : CreateParticipantDto(replyMessageLastEditedBy!),
            replyMessage.Deleted, replyMessage.DeletedById is null ? null : CreateParticipantDto(replyMessageDeletedBy!));
    }

    public MessageDto CreateMessageDto(Message message, ChannelParticipant postedBy, ChannelParticipant? editedBy, ChannelParticipant? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions)
    {
        return new MessageDto(
            message.Id,
            message.ChannelId,
            replyMessageDto,
            message.Content,
            message.Posted, CreateParticipantDto(postedBy),
            message.LastEdited, message.LastEditedById is null ? null : CreateParticipantDto(editedBy!),
            message.Deleted, message.DeletedById is null ? null : CreateParticipantDto(deletedBy!),
            reactions);
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }

    public ParticipantDto CreateParticipantDto(ChannelParticipant participant)
    {
        return new ParticipantDto(participant!.Id, participant.ChannelId, participant.DisplayName ?? string.Empty, participant.UserId);
    }

    public ReactionDto CreateReactionDto(MessageReaction reaction, ChannelParticipant addedBy)
    {
        return new ReactionDto(reaction.Reaction, reaction.Date, CreateParticipantDto(addedBy));
    }
}