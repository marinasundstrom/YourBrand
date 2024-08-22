using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;

using static YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record PostMessage(Guid ChannelId, Guid? ReplyToId, string Content) : IRequest<Result<MessageId>>
{
    public sealed class Validator : AbstractValidator<PostMessage>
    {
        public Validator()
        {
            // RuleFor(x => x.Content).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler(
        IUserContext userContext,
        ApplicationDbContext applicationDbContext,
        IChannelRepository channelRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IAdminCommandProcessor adminCommandProcessor) : IRequestHandler<PostMessage, Result<MessageId>>
    {
        public async Task<Result<MessageId>> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var notification = request;

            var hasChannel = await channelRepository
                .GetAll(new ChannelWithId(request.ChannelId))
                .AnyAsync(cancellationToken);

            if (!hasChannel)
            {
                return ChannelNotFound;
            }

            if (IsAdminCommand(notification.Content, out var args))
            {
                return await adminCommandProcessor.ProcessAdminCommand(request.ChannelId.ToString(), args, cancellationToken);
            }

            Message message;

            if (request.ReplyToId is not null)
            {
                message = new Message(request.ChannelId, request.ReplyToId.GetValueOrDefault(), request.Content);
            }
            else
            {
                message = new Message(request.ChannelId, request.Content);
            }

            message.PostedBy = await applicationDbContext.ChannelParticipants
                .FirstOrDefaultAsync(x => x.UserId == userContext.UserId, cancellationToken);

            messageRepository.Add(message);

            var channel = await channelRepository.FindByIdAsync(message.ChannelId, cancellationToken);

            var participant = channel.Participants.FirstOrDefault(x => x.UserId == x.UserId);

            message.Posted = DateTimeOffset.UtcNow;
            message.PostedById = participant.Id;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return message.Id;
        }

        private bool IsAdminCommand(string message, out string[] args)
        {
            var args0 = message.Split(' ');

            if (args0.Any() && args0[0].Equals("/admin"))
            {
                args = args0;
                return true;
            }

            args = Array.Empty<string>();
            return false;
        }
    }
}