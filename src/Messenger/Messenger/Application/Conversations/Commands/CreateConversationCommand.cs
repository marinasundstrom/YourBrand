
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
    public class CreateConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<CreateConversationCommand, ConversationDto>
    {
        public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = userContext.UserId;

            var conversation = new Conversation()
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title
            };

            conversationRepository.AddConversation(conversation);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            //await _mediator.Send(new JoinConversationCommand(conversation.Id), cancellationToken);

            conversation = await conversationRepository.GetConversation(conversation.Id);
            return conversation!.ToDto();
        }
    }
}