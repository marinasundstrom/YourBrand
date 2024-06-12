using FluentValidation;
using MediatR;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

public sealed record RemoveReaction(Guid MessageId, string Reaction) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<RemoveReaction>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Reaction).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<RemoveReaction, Result>
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

        public async Task<Result> Handle(RemoveReaction request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = currentUserService.UserId!;

            message.RemoveReaction(userId, request.Reaction);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}