
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Conversations.Queries;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Entities;

using MassTransit;

using MediatR;

namespace YourBrand.Messenger.Application.Conversations.Commands;

public record CreateConversationCommand(string? Title) : IRequest<ConversationDto>
{
    public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
    {
        private readonly IMediator _mediator;
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public CreateConversationCommandHandler(IMediator mediator, IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            _mediator = mediator;
            this.context = context;
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

            context.Conversations.Add(conversation);

            await context.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new JoinConversationCommand(conversation.Id), cancellationToken);

            return await _mediator.Send(new GetConversationQuery(conversation.Id), cancellationToken)!;
        }
    }
}
