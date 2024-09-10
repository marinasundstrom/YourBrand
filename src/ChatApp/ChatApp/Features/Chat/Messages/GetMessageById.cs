using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;

using static YourBrand.ChatApp.Domain.Errors.Messages;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public record GetMessageById(OrganizationId OrganizationId, ChannelId ChannelId, MessageId Id) : IRequest<Result<MessageDto>>
{
    public class Validator : AbstractValidator<GetMessageById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(ApplicationDbContext applicationDbContext, IDtoComposer dtoComposer) : IRequestHandler<GetMessageById, Result<MessageDto>>
    {
        public async Task<Result<MessageDto>> Handle(GetMessageById request, CancellationToken cancellationToken)
        {
            var message = await applicationDbContext
                .Messages
                .InOrganization(request.OrganizationId)
                .InChannel(request.ChannelId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (message is null)
            {
                return MessageNotFound;
            }

            return await dtoComposer.ComposeMessageDto(message, cancellationToken);
        }
    }
}