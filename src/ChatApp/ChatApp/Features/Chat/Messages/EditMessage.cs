using FluentValidation;
using MediatR;
using YourBrand.ChatApp.Domain;

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

    public sealed class Handler : IRequestHandler<EditMessage, Result>
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserContext userContext;

        public Handler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IUserContext userContext)
        {
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
            this.userContext = userContext;
        }

        public async Task<Result> Handle(EditMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = userContext.UserId;

            if(message.CreatedById != userId) 
            {
                return Result.Failure(Errors.Messages.NotAllowedToEdit);
            }

            message.UpdateContent(request.Content);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
