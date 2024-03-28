
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Conversations.Queries;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Entities;

using MassTransit;

using MediatR;
using YourBrand.Identity;
using YourBrand.Messenger.Domain.Repositories;
using YourBrand.Messenger.Domain;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record CreateConversationCommand(string? Title) : IRequest<ConversationDto>
{
    public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public CreateConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBus bus)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var conversation = new Conversation() {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title
            };

            _conversationRepository.AddConversation(conversation);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //await _mediator.Send(new JoinConversationCommand(conversation.Id), cancellationToken);

            conversation = await _conversationRepository.GetConversation(conversation.Id);
            return conversation!.ToDto();
        }
    }
}
