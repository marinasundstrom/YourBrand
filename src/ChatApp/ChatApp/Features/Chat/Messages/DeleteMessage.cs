using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;

using static YourBrand.ChatApp.Domain.Errors.Messages;


namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record DeleteMessage(OrganizationId OrganizationId, ChannelId ChannelId, MessageId MessageId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<DeleteMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUserContext userContext) : IRequestHandler<DeleteMessage, Result>
    {
        public async Task<Result> Handle(DeleteMessage request, CancellationToken cancellationToken)
        {
            var message = await applicationDbContext
                .Messages
                .InOrganization(request.OrganizationId)
                .InChannel(request.ChannelId)
                .FirstOrDefaultAsync(x => x.Id == request.MessageId, cancellationToken);

            if (message is null)
            {
                return MessageNotFound;
            }

            var channel = await applicationDbContext
                .Channels
                .InOrganization(request.OrganizationId)
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);

            var userId = userContext.UserId;
            var isAdmin = userContext.IsInRole("admin");

            if (!isAdmin && message.PostedBy!.UserId != userId)
            {
                return NotAllowedToDelete;
            }

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
                applicationDbContext.Messages.Remove(message);
            }

            await applicationDbContext.SaveChangesAsync(cancellationToken);

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