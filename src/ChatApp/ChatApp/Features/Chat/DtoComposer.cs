using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;

namespace YourBrand.ChatApp.Features.Chat;

public interface IDtoComposer
{
    Task<MessageDto> ComposeMessageDto(Message message, CancellationToken cancellationToken = default);
    Task<IEnumerable<MessageDto>> ComposeMessageDtos(Message[] messages, CancellationToken cancellationToken = default);
}

public sealed class DtoComposer : IDtoComposer
{
    private readonly ApplicationDbContext context;
    private readonly IDtoFactory dtoFactory;

    public DtoComposer(ApplicationDbContext context, IDtoFactory dtoFactory)
    {
        this.context = context;
        this.dtoFactory = dtoFactory;
    }

    public async Task<MessageDto> ComposeMessageDto(Message message, CancellationToken cancellationToken = default)
    {
        HashSet<ChannelParticipantId> participantIds = new();
        HashSet<MessageId> messageIds = new();

        ExtractChannelParticipantIds(message, participantIds);
        ExtractReplyIds(message, messageIds);

        var replyMessages = await context.Messages
            .IgnoreQueryFilters()
            .Where(x => messageIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        if (replyMessages.Any())
        {
            foreach (var replyMessage in replyMessages.Select(x => x.Value))
            {
                ExtractChannelParticipantIds(replyMessage, participantIds);
            }
        }

        var participants = await context.ChannelParticipants
            .Where(x => x.ChannelId == message.ChannelId)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return ComposeMessageDtoInternal(message, participantIdUsers, replyMessages);
    }

    private static void ExtractReplyIds(Message message, HashSet<MessageId> messageIds)
    {
        if (message.ReplyToId is not null)
        {
            messageIds.Add(message.ReplyToId.GetValueOrDefault());
        }
    }

    private static void ExtractChannelParticipantIds(Message message, HashSet<ChannelParticipantId> participantIds)
    {
        if (message.PostedById is not null)
        {
            participantIds.Add(message.PostedById.GetValueOrDefault());
        }

        if (message.LastEditedById is not null)
        {
            participantIds.Add(message.LastEditedById.GetValueOrDefault());
        }

        if (message.DeletedById is not null)
        {
            participantIds.Add(message.DeletedById.GetValueOrDefault());
        }

        foreach (var reaction in message.Reactions)
        {
            participantIds.Add(reaction.AddedById);
        }
    }

    public async Task<IEnumerable<MessageDto>> ComposeMessageDtos(Message[] messages, CancellationToken cancellationToken = default)
    {
        HashSet<ChannelParticipantId> participantIds = new();
        HashSet<MessageId> messageIds = new();

        foreach (var message in messages)
        {
            ExtractChannelParticipantIds(message, participantIds);
            ExtractReplyIds(message, messageIds);
        }

        var replyMessages = await context.Messages
            .IgnoreQueryFilters()
            .Where(x => messageIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        if (replyMessages.Any())
        {
            foreach (var replyMessage in replyMessages.Select(x => x.Value))
            {
                ExtractChannelParticipantIds(replyMessage, participantIds);
            }
        }

        var participants = await context.ChannelParticipants
            //.Where(x => x.ChannelId == message.ChannelId)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return messages.Select(message =>
        {
            return ComposeMessageDtoInternal(message, participantIdUsers, replyMessages);
        });
    }

    private MessageDto ComposeMessageDtoInternal(Message message, Dictionary<ChannelParticipantId, User> users, Dictionary<MessageId, Message> repliedMessages)
    {
        repliedMessages.TryGetValue(message.ReplyToId.GetValueOrDefault(), out var replyMessage);

        users.TryGetValue(message.PostedById.GetValueOrDefault(), out var publishedBy);

        users.TryGetValue(message.LastEditedById.GetValueOrDefault(), out var editedBy);

        users.TryGetValue(message.DeletedById.GetValueOrDefault(), out var deletedBy);

        ReplyMessageDto? replyMessageDto = null;

        if (replyMessage is not null)
        {
            users.TryGetValue(replyMessage.PostedById.GetValueOrDefault(), out var replyMessagePublishedBy);

            users.TryGetValue(replyMessage.LastEditedById.GetValueOrDefault(), out var replyMessageEditedBy);

            users.TryGetValue(replyMessage.DeletedById.GetValueOrDefault(), out var replyMessageDeletedBy);

            replyMessageDto = dtoFactory.CreateReplyMessageDto(replyMessage, replyMessagePublishedBy!, replyMessageEditedBy, replyMessageDeletedBy);
        }

        var reactions = message.Reactions.Select(x => dtoFactory.CreateReactionDto(x, users[x.AddedById], x.AddedById.ToString()));

        return dtoFactory.CreateMessageDto(message, publishedBy!, editedBy, deletedBy, replyMessageDto, reactions);
    }
}