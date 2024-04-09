
using MediatR;

using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Queries;

public record GetConversationQuery(string Id) : IRequest<ConversationDto?>
{
    public class GetConversationQueryHandler(IConversationRepository conversationRepository) : IRequestHandler<GetConversationQuery, ConversationDto?>
    {
        public async Task<ConversationDto?> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var conversation = await conversationRepository.GetConversation(request.Id!, cancellationToken);

            if (conversation is null) return null;

            return conversation.ToDto();
        }
    }
}