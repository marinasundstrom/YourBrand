using FluentValidation;
using MediatR;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

public record GetMessageById(Guid Id) : IRequest<Result<MessageDto>>
{
    public class Validator : AbstractValidator<GetMessageById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<GetMessageById, Result<MessageDto>>
    {
        private readonly IMessageRepository messageRepository;
        private readonly IDtoComposer dtoComposer;

        public Handler(IMessageRepository messageRepository, IDtoComposer dtoComposer)
        {
            this.messageRepository = messageRepository;
            this.dtoComposer = dtoComposer;
        }

        public async Task<Result<MessageDto>> Handle(GetMessageById request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.Id, cancellationToken);

            if (message is null)
            {
                return Result.Failure<MessageDto>(Errors.Messages.MessageNotFound);
            }

            return Result.Success(
                await dtoComposer.ComposeMessageDto(message, cancellationToken));
        }
    }
}