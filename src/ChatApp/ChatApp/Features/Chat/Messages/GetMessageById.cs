using FluentValidation;

using MediatR;

using YourBrand.ChatApp.Domain;

using static YourBrand.ChatApp.Domain.Errors.Messages;


namespace YourBrand.ChatApp.Features.Chat.Messages;

public record GetMessageById(Guid Id) : IRequest<Result<MessageDto>>
{
    public class Validator : AbstractValidator<GetMessageById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(IMessageRepository messageRepository, IDtoComposer dtoComposer) : IRequestHandler<GetMessageById, Result<MessageDto>>
    {
        public async Task<Result<MessageDto>> Handle(GetMessageById request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.Id, cancellationToken);

            if (message is null)
            {
                return MessageNotFound;
            }

            return await dtoComposer.ComposeMessageDto(message, cancellationToken);
        }
    }
}