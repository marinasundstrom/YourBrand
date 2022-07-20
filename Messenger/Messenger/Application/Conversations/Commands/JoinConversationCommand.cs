
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Entities;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Identity;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record JoinConversationCommand(string? ConversationId) : IRequest
{
    public class JoinConversationCommandHandler : IRequestHandler<JoinConversationCommand>
    {
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public JoinConversationCommandHandler(IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            this.context = context;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<Unit> Handle(JoinConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var conversation = await context.Conversations.FirstOrDefaultAsync(x => x.Id == request.ConversationId, cancellationToken);

            if(conversation is null)
            {
                throw new Exception();
            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user is null)
            {
                throw new Exception();
            }

            conversation.AddParticipant(user);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
