using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;

using Errors = YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public sealed record CreateChannel(string Name) : IRequest<Result<ChannelDto>>
{
    public sealed class Validator : AbstractValidator<CreateChannel>
    {
        public Validator()
        {
            // RuleFor(x => x.Content).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Name).MaximumLength(1024);
        }
    }

    public sealed class Handler(
        IChannelRepository channelRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext) : IRequestHandler<CreateChannel, Result<ChannelDto>>
    {
        public async Task<Result<ChannelDto>> Handle(CreateChannel request, CancellationToken cancellationToken)
        {
            var hasChannel = await channelRepository
                .GetAll(new ChannelWithName(request.Name))
                .AnyAsync(cancellationToken);

            if (hasChannel)
            {
                return Errors.ChannelWithNameAlreadyExists;
            }

            var channel = new Channel(request.Name);

            channelRepository.Add(channel);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return channel.ToDto();
        }
    }
}