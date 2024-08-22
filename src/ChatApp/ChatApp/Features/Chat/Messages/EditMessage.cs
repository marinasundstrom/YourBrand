using FluentValidation;

using MediatR;

using YourBrand.ChatApp.Domain;

using static YourBrand.ChatApp.Domain.Errors.Messages;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record EditMessage(Guid MessageId, string Content) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<EditMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Content).MaximumLength(1024);
        }
    }

    public sealed class Handler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<EditMessage, Result>
    {
        public async Task<Result> Handle(EditMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

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

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}