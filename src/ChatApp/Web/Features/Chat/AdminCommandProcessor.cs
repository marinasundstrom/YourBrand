using ChatApp.Domain;
using ChatApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Features.Chat;

public interface IAdminCommandProcessor
{
    Task<Result<MessageId>> ProcessAdminCommand(string channelId, string[] args, CancellationToken cancellationToken);
}

public sealed class AdminCommandProcessor : IAdminCommandProcessor
{
    private readonly IMessageRepository messageRepository;
    private readonly ICurrentUserService currentUserService;
    private readonly IChatNotificationService chatNotificationService;

    public AdminCommandProcessor(IMessageRepository messageRepository, ICurrentUserService currentUserService, IChatNotificationService chatNotificationService)
    {
        this.messageRepository = messageRepository;
        this.currentUserService = currentUserService;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task<Result<MessageId>> ProcessAdminCommand(string channelId, string[] args, CancellationToken cancellationToken)
    {
        if (args.Length >= 2 && args[1].Equals("getNumberPosts"))
        {
            string content;

            if (args.Length == 3 && args[2].Equals("/channel"))
            {
                var numberOfPosts = await messageRepository.GetAll(new MessagesInChannel(Guid.Parse(channelId))).CountAsync(cancellationToken);

                content = $"Number of posts in channel: {numberOfPosts}";
            }
            else
            {
                var numberOfPosts = await messageRepository.GetAll().CountAsync(cancellationToken);

                content = $"Total number of posts: {numberOfPosts}";
            }

            MessageDto messageDto = CreateMessage(channelId, content);

            await SendMessage(messageDto, cancellationToken);
        }
        else if (args.Length > 1)
        {
            var content = $"Unknown command";

            MessageDto messageDto = CreateMessage(channelId, content);

            await SendMessage(messageDto, cancellationToken);
        }
        else
        {
            var content = $"Command expected";

            MessageDto messageDto = CreateMessage(channelId, content);

            await SendMessage(messageDto, cancellationToken);
        }

        return Result.Success(new MessageId());
    }

    private static MessageDto CreateMessage(string channelId, string content)
    {
        return new MessageDto(Guid.NewGuid(), Guid.Parse(channelId), null, content, DateTimeOffset.UtcNow, new Users.UserDto("system", "System"), null, null, null, null, Enumerable.Empty<ReactionDto>());
    }

    private async Task SendMessage(MessageDto messageDto, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendMessageToUser(
            currentUserService.UserId!.ToString(), messageDto, cancellationToken);
    }
}