using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;

using Errors = YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public sealed record JoinChannel(ChannelId ChannelId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<JoinChannel>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IChannelRepository channelRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<JoinChannel, Result>
    {
        public async Task<Result> Handle(JoinChannel request, CancellationToken cancellationToken)
        {
            var channel = await channelRepository.FindByIdAsync(request.ChannelId, cancellationToken);

            if (channel is null)
            {
                return Errors.ChannelNotFound;
            }

            var userId = userContext.UserId.GetValueOrDefault();

            channel = await applicationDbContext
                .Channels.Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.ChannelId, cancellationToken);

            channel.AddParticipant(userId);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}