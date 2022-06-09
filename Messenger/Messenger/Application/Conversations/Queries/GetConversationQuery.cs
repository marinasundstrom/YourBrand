
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Conversations.Queries;

public record GetConversationQuery(string Id) : IRequest<ConversationDto?>
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, ConversationDto?>
    {
        private readonly IMessengerContext context;

        public GetConversationQueryHandler(IMessengerContext context)
        {
            this.context = context;
        }

        public async Task<ConversationDto?> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var conversation = await context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (conversation is null) return null;

            return conversation.ToDto();
        }
    }
}