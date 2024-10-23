using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;

using static YourBrand.ChatApp.Domain.Errors.Messages;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record React(OrganizationId OrganizationId, ChannelId ChannelId, MessageId MessageId, string Reaction) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<React>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Reaction).MaximumLength(1024);
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUserContext userContext) : IRequestHandler<React, Result>
    {
        public async Task<Result> Handle(React request, CancellationToken cancellationToken)
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

            var userId = userContext.UserId.GetValueOrDefault();

            var channel = await applicationDbContext
                .Channels
                .InOrganization(request.OrganizationId)
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);

            var participant = channel.Participants.FirstOrDefault(x => x.UserId == userId);

            message.React(participant.Id, request.Reaction);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}