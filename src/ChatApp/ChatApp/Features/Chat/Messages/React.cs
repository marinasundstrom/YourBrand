using FluentValidation;
using MediatR;
using YourBrand.ChatApp.Domain;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record React(Guid MessageId, string Reaction) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<React>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();

            RuleFor(x => x.Reaction).MaximumLength(1024);
        }
    }

    public sealed class Handler : IRequestHandler<React, Result>
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

        public async Task<Result> Handle(React request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if (message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = userContext.UserId.GetValueOrDefault();

            message.React(userId, request.Reaction);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
