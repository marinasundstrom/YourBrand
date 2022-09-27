
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Entities;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Identity;
using YourBrand.Messenger.Domain.Repositories;
using YourBrand.Messenger.Domain;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record JoinConversationCommand(string? ConversationId) : IRequest
{
    public class JoinConversationCommandHandler : IRequestHandler<JoinConversationCommand>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public JoinConversationCommandHandler(IConversationRepository conversationRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBus bus)
        {
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<Unit> Handle(JoinConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var conversation = await _conversationRepository.GetConversation(request.ConversationId!, cancellationToken);

            if(conversation is null)
            {
                throw new Exception();
            }

            var user = await _userRepository.GetUserById(userId!, cancellationToken);

            if (user is null)
            {
                throw new Exception();
            }

            conversation.AddParticipant(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
