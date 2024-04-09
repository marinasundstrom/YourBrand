
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record LeaveConversationCommand(string? ConversationId) : IRequest
{
    public sealed class LeaveConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<LeaveConversationCommand>
    {
        public async Task Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
        {
            var conversation = await conversationRepository.GetConversation(request.ConversationId!, cancellationToken);

            if (conversation is null)
            {
                throw new Exception();
            }

            var participant = conversation.Participants.FirstOrDefault(x => x.Id == userContext.UserId);

            if (participant is null)
            {
                throw new Exception();
            }

            conversation.RemoveParticipant(participant);

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}