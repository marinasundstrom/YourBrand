
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record JoinConversationCommand(string? ConversationId) : IRequest
{
    public class JoinConversationCommandHandler(IConversationRepository conversationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<JoinConversationCommand>
    {
        public async Task Handle(JoinConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = userContext.UserId;

            var conversation = await conversationRepository.GetConversation(request.ConversationId!, cancellationToken);

            if (conversation is null)
            {
                throw new Exception();
            }

            var user = await userRepository.GetUserById(userId!, cancellationToken);

            if (user is null)
            {
                throw new Exception();
            }

            conversation.AddParticipant(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}