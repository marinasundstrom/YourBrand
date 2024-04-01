
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record LeaveConversationCommand(string? ConversationId) : IRequest
{
    public class LeaveConversationCommandHandler : IRequestHandler<LeaveConversationCommand>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public LeaveConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBus bus)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var conversation = await _conversationRepository.GetConversation(request.ConversationId!, cancellationToken);

            if (conversation is null)
            {
                throw new Exception();
            }

            var participant = conversation.Participants.FirstOrDefault(x => x.Id == request.ConversationId);

            if (participant is null)
            {
                throw new Exception();
            }

            conversation.RemoveParticipant(participant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}