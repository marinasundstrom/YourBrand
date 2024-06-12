using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain;

namespace ChatApp.Features.Chat.Messages;

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
        private readonly ICurrentUserService currentUserService;

        public Handler(
            IMessageRepository messageRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteMessage request, CancellationToken cancellationToken)
        {
            var message = await messageRepository.FindByIdAsync(request.MessageId, cancellationToken);

            if(message is null)
            {
                return Result.Failure(Errors.Messages.MessageNotFound);
            }

            var userId = currentUserService.UserId;
            var isAdmin = currentUserService.IsInRole("admin");

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
