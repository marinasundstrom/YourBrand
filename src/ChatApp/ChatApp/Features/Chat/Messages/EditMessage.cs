using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;

using static YourBrand.ChatApp.Domain.Errors.Messages;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record EditMessage(OrganizationId OrganizationId, ChannelId ChannelId, MessageId MessageId, string Content) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<EditMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUserContext userContext) : IRequestHandler<EditMessage, Result>
    {
        public async Task<Result> Handle(EditMessage request, CancellationToken cancellationToken)
        {
            var message = await applicationDbContext
                .Messages
                .InOrganization(request.OrganizationId)
                .InChannel(request.ChannelId)
                .FirstOrDefaultAsync(x => x.Id == request.MessageId,cancellationToken);

            if (message is null)
            {
                return MessageNotFound;
            }

            var userId = userContext.UserId;

            if (message.PostedBy?.UserId != userId)
            {
                return NotAllowedToEdit;
            }

            message.UpdateContent(request.Content);

            var channel = await applicationDbContext
                .Channels
                .InOrganization(request.OrganizationId)
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);
                
            var participant = channel.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            message.LastEdited = DateTimeOffset.UtcNow;
            message.LastEditedById = participant.Id;

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}