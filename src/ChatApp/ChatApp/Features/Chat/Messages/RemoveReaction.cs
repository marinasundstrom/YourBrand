using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Identity;

using static YourBrand.ChatApp.Domain.Errors.Messages;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record RemoveReaction(OrganizationId OrganizationId, ChannelId ChannelId, MessageId MessageId, string Reaction) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<RemoveReaction>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Reaction).MaximumLength(1024);
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUserContext userContext) : IRequestHandler<RemoveReaction, Result>
    {
        public async Task<Result> Handle(RemoveReaction request, CancellationToken cancellationToken)
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

            var userId = userContext.UserId.GetValueOrDefault();

            var channel = await applicationDbContext.Channels.Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == message.ChannelId, cancellationToken);

            var participant = channel.Participants.FirstOrDefault(x => x.UserId == userId);

            message.RemoveReaction(participant.Id, request.Reaction);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}