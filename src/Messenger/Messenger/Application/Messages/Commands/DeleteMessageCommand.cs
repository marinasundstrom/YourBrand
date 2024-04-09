
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record DeleteMessageCommand(string ConversationId, string MessageId) : IRequest
{
    public class DeleteMessageCommandHandler(IConversationRepository conversationRepository, IMessageRepository messageRepository, IUnitOfWork unitOfWork, IUserContext userContext, IBus bus) : IRequestHandler<DeleteMessageCommand>
    {
        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await conversationRepository.GetConversation(request.ConversationId, cancellationToken);

            if (conversation is null) throw new Exception();

            var message = await messageRepository.GetMessage(request.MessageId, cancellationToken);

            if (message is null) throw new Exception();

            if (!IsAuthorizedToDelete(message))
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }

            conversation.DeleteMessage(message);

            //_messageRepository.DeleteMessage(message);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await bus.Publish(new MessageDeleted(null!, message.Id));

        }

        private bool IsAuthorizedToDelete(Domain.Entities.Message message) => userContext.IsCurrentUser(message.CreatedById!) || userContext.IsUserInRole(Roles.Administrator);
    }
}