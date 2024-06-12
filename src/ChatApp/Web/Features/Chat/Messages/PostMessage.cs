using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;
using ChatApp.Domain.ValueObjects;

using Microsoft.Extensions.Caching.Distributed;
namespace ChatApp.Features.Chat.Messages;

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

    public sealed class Handler : IRequestHandler<PostMessage, Result<MessageId>>
    {
        private readonly IChannelRepository channelRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAdminCommandProcessor adminCommandProcessor;

        public Handler(
            IChannelRepository channelRepository,
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            IAdminCommandProcessor adminCommandProcessor)
        {
            this.channelRepository = channelRepository;
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
            this.adminCommandProcessor = adminCommandProcessor;
        }

        public async Task<Result<MessageId>> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var notification = request;

            var hasChannel = await channelRepository
                .GetAll(new ChannelWithId(request.ChannelId))
                .AnyAsync(cancellationToken);

            if (!hasChannel)
            {
                return Result.Failure<MessageId>(Errors.Channels.ChannelNotFound);
            }

            if (IsAdminCommand(notification.Content, out var args))
            {
                return await adminCommandProcessor.ProcessAdminCommand(request.ChannelId.ToString(), args, cancellationToken);
            }

            Message message;
            
            if(request.ReplyToId is not null) 
            { 
                message = new Message(request.ChannelId, request.ReplyToId.GetValueOrDefault(), request.Content); 
            } 
            else 
            {
                message = new Message(request.ChannelId, request.Content); 
            } 
            
            messageRepository.Add(message);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(message.Id);
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