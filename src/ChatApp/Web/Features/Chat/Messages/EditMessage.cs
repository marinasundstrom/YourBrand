using FluentValidation;
using MediatR;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

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
        private readonly ICurrentUserService currentUserService;

        public Handler(IMessageRepository messageRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.messageRepository = messageRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result> Handle(EditMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = currentUserService.UserId;

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
