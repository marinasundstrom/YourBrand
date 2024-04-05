
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain;
using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record CreateConversationCommand(string? Title) : IRequest<ConversationDto>
{
    public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IBus _bus;

        public CreateConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, IUserContext userContext, IBus bus)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _bus = bus;
        }

        public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;

            var conversation = new Conversation()
            {
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