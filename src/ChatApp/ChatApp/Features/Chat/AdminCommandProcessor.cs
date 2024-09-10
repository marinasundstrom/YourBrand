using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Chat.Channels;

namespace YourBrand.ChatApp.Features.Chat;

public interface IAdminCommandProcessor
{
    Task<Result<string>> ProcessAdminCommand(string channelId, string[] args, CancellationToken cancellationToken);
}

public sealed class AdminCommandProcessor : IAdminCommandProcessor
{
    private readonly IMessageRepository messageRepository;
    private readonly IUserContext userContext;
    private readonly IChatNotificationService chatNotificationService;

    public AdminCommandProcessor(IMessageRepository messageRepository, IUserContext userContext, IChatNotificationService chatNotificationService)
    {
        this.messageRepository = messageRepository;
        this.userContext = userContext;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task<Result<string>> ProcessAdminCommand(string channelId, string[] args, CancellationToken cancellationToken)
    {
        if (args.Length >= 2 && args[1].Equals("getNumberPosts"))
        {
            string content;

            if (args.Length == 3 && args[2].Equals("/channel"))
            {
                var numberOfPosts = await messageRepository.GetAll(new MessagesInChannel(channelId)).CountAsync(cancellationToken);

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

        return Result.SuccessWith(string.Empty);
    }

    private static MessageDto CreateMessage(string channelId, string content)
    {
        return new MessageDto("", channelId, null, content, DateTimeOffset.UtcNow, new ParticipantDto("", channelId, "System", null), null, null, null, null, Enumerable.Empty<ReactionDto>());
    }

    private async Task SendMessage(MessageDto messageDto, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendMessageToUser(
            userContext.UserId.GetValueOrDefault(), messageDto, cancellationToken);
    }
}