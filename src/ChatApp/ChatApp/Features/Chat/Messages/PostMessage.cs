using FluentValidation;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.Specifications;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;

using static YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record PostMessage(OrganizationId OrganizationId, ChannelId ChannelId, MessageId? ReplyToId, string Content) : IRequest<Result<string>>
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
        IAdminCommandProcessor adminCommandProcessor,
        IPublishEndpoint publisher) : IRequestHandler<PostMessage, Result<string>>
    {
        public async Task<Result<string>> Handle(PostMessage request, CancellationToken cancellationToken)
        {
            var notification = request;

            var channel = await applicationDbContext.Channels
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);

            if (channel is null)
            {
                return ChannelNotFound;
            }

            if (IsAdminCommand(notification.Content, out var args))
            {
                return await adminCommandProcessor.ProcessAdminCommand(request.ChannelId.ToString(), args, cancellationToken);
            }

            string id = Guid.NewGuid().ToString();

            await publisher.Publish(new YourBrand.ChatApp.Consumers.PostMessage2(id, notification.OrganizationId, notification.ChannelId, notification.ReplyToId, notification.Content));
            return id;
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