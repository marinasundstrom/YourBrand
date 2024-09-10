using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

public interface IDtoFactory
{
    ReplyMessageDto CreateReplyMessageDto(Message replyMessage, ChannelParticipant replyMessagePublishedBy, ChannelParticipant? replyMessageEditedBy, ChannelParticipant? replyMessageDeletedBy, Dictionary<ChannelParticipantId, User> users);
    MessageDto CreateMessageDto(Message message, ChannelParticipant publishedBy, ChannelParticipant? editedBy, ChannelParticipant? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions, Dictionary<ChannelParticipantId, User> users);
    ParticipantDto CreateParticipantDto(ChannelParticipant participant, Dictionary<ChannelParticipantId, User> users);
    UserDto CreateUserDto(User user);
    ReactionDto CreateReactionDto(MessageReaction reaction, ChannelParticipant addedBy, Dictionary<ChannelParticipantId, User> users);
}

public sealed class DtoFactory : IDtoFactory
{
    public ReplyMessageDto CreateReplyMessageDto(Message replyMessage, ChannelParticipant replyMessagePostedBy, ChannelParticipant? replyMessageLastEditedBy, ChannelParticipant? replyMessageDeletedBy, Dictionary<ChannelParticipantId, User> users)
    {
        return new ReplyMessageDto(
            replyMessage.Id,
            replyMessage.ChannelId,
            replyMessage.Content,
            replyMessage.Posted, CreateParticipantDto(replyMessagePostedBy, users),
            replyMessage.LastEdited, replyMessage.LastEditedById is null ? null : CreateParticipantDto(replyMessageLastEditedBy!, users),
            replyMessage.Deleted, replyMessage.DeletedById is null ? null : CreateParticipantDto(replyMessageDeletedBy!, users));
    }

    public MessageDto CreateMessageDto(Message message, ChannelParticipant postedBy, ChannelParticipant? editedBy, ChannelParticipant? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions, Dictionary<ChannelParticipantId, User> users)
    {
        return new MessageDto(
            message.Id,
            message.ChannelId,
            replyMessageDto,
            message.Content,
            message.Posted, CreateParticipantDto(postedBy, users),
            message.LastEdited, message.LastEditedById is null ? null : CreateParticipantDto(editedBy!, users),
            message.Deleted, message.DeletedById is null ? null : CreateParticipantDto(deletedBy!, users),
            reactions);
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }

    public ParticipantDto CreateParticipantDto(ChannelParticipant participant, Dictionary<ChannelParticipantId, User> users)
    {
        return new ParticipantDto(
            participant!.Id,
            participant.ChannelId,
            participant.DisplayName ?? users[participant.Id].Name,
            participant.UserId);
    }

    public ReactionDto CreateReactionDto(MessageReaction reaction, ChannelParticipant addedBy, Dictionary<ChannelParticipantId, User> users)
    {
        return new ReactionDto(reaction.Reaction, reaction.Date, CreateParticipantDto(addedBy, users));
    }
}