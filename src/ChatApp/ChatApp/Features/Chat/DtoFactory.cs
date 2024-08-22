using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

public interface IDtoFactory
{
    ReplyMessageDto CreateReplyMessageDto(Message replyMessage, User replyMessagePublishedBy, User? replyMessageEditedBy, User? replyMessageDeletedBy);
    MessageDto CreateMessageDto(Message message, User publishedBy, User? editedBy, User? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions);
    UserDto CreateUserDto(User user);
    ReactionDto CreateReactionDto(MessageReaction reaction, User user);
}

public sealed class DtoFactory : IDtoFactory
{
    public ReplyMessageDto CreateReplyMessageDto(Message replyMessage, User replyMessagePostedBy, User? replyMessageLastEditedBy, User? replyMessageDeletedBy)
    {
        return new ReplyMessageDto(
            (Guid)replyMessage.Id,
            replyMessage.ChannelId,
            replyMessage.Content,
            replyMessage.Posted, CreateUserDto(replyMessagePostedBy),
            replyMessage.LastEdited, replyMessage.LastEditedById is null ? null : CreateUserDto(replyMessageLastEditedBy!),
            replyMessage.Deleted, replyMessage.DeletedById is null ? null : CreateUserDto(replyMessageDeletedBy!));
    }

    public MessageDto CreateMessageDto(Message message, User postedBy, User? editedBy, User? deletedBy, ReplyMessageDto? replyMessageDto, IEnumerable<ReactionDto> reactions)
    {
        return new MessageDto(
            message.Id,
            message.ChannelId,
            replyMessageDto,
            message.Content,
            message.Posted, CreateUserDto(postedBy),
            message.LastEdited, message.LastEditedById is null ? null : CreateUserDto(editedBy!),
            message.Deleted, message.DeletedById is null ? null : CreateUserDto(deletedBy!),
            reactions);
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }

    public ReactionDto CreateReactionDto(MessageReaction reaction, User user)
    {
        return new ReactionDto(reaction.Reaction, reaction.Date, CreateUserDto(user));
    }
}