using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourBrand.ChatApp.Domain;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public sealed record DeleteMessage(Guid MessageId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<DeleteMessage>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteMessage, Result>
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserContext userContext;

        public Handler(
            IMessageRepository messageRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.userContext = userContext;
        }

        public async Task<Result> Handle(DeleteMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if(message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = userContext.UserId;
            var isAdmin = userContext.IsInRole("admin");

            if(!isAdmin && message.CreatedById != userId) 
            {
                return Result.Failure(Errors.Messages.NotAllowedToDelete);
            }

            message.RemoveAllReactions();

            message.MarkAsDeleted();

            messageRepository.Remove(message);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

            /*
            var deleted = await messageRepository
                .GetAll(new MessageWithId(request.MessageId))
                .ExecuteDeleteAsync();      

            return deleted > 0 
                ? Result.Success() 
                : Result.Failure(Errors.Messages.MessageNotFound);
            */
        }
    }
}
