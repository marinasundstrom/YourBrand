using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;

using static YourBrand.ChatApp.Domain.Errors.Messages;


namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record DeleteMessage(Guid MessageId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<DeleteMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
        }
    }

    public sealed class Handler(
        IChannelRepository channelRepository,
        IMessageRepository messageRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<DeleteMessage, Result>
    {
        public async Task<Result> Handle(DeleteMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return MessageNotFound;
            }

            var userId = userContext.UserId;
            var isAdmin = userContext.IsInRole("admin");

            if (!isAdmin && message.PostedBy!.UserId != userId)
            {
                return NotAllowedToDelete;
            }

            var channel = await channelRepository.FindByIdAsync(message.ChannelId, cancellationToken);
            
            var shouldSoftDelete = channel.Settings.SoftDeleteMessages.GetValueOrDefault();

            message.RemoveAllReactions();

            message.MarkAsDeleted();

            if (shouldSoftDelete) 
            {
                var participant = channel.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

                message.Deleted = DateTimeOffset.UtcNow;
                message.DeletedById = participant.Id;
            }
            else 
            {
                messageRepository.Remove(message);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;

            /*
            var deleted = await messageRepository
                .GetAll(new MessageWithId(request.MessageId))
                .ExecuteDeleteAsync();      

            return deleted > 0 
                ? Result.Success() 
                : Result.Failure(Errors.Messages.MessageNotFound);
            */
        }
    }
}