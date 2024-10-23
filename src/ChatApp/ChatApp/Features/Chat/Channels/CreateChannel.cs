using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;

using Errors = YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public sealed record CreateChannel(OrganizationId OrganizationId, string Name) : IRequest<Result<ChannelDto>>
{
    public sealed class Validator : AbstractValidator<CreateChannel>
    {
        public Validator()
        {
            // RuleFor(x => x.Content).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Name).MaximumLength(1024);
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : IRequestHandler<CreateChannel, Result<ChannelDto>>
    {
        public async Task<Result<ChannelDto>> Handle(CreateChannel request, CancellationToken cancellationToken)
        {
            var channel = new Channel(request.OrganizationId, request.Name);

            applicationDbContext.Channels.Add(channel);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return channel.ToDto();
        }
    }
}