
using YourBrand.Messenger.Application.Common.Interfaces;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record LeaveConversationCommand(string? ConversationId) : IRequest
{
    public class LeaveConversationCommandHandler : IRequestHandler<LeaveConversationCommand>
    {
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public LeaveConversationCommandHandler(IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            this.context = context;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<Unit> Handle(LeaveConversationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var participant = await context.ConversationParticipants.FirstOrDefaultAsync(x => x.Id == request.ConversationId, cancellationToken);

            if (participant is null)
            {
                throw new Exception();
            }

            context.ConversationParticipants.Remove(participant);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
