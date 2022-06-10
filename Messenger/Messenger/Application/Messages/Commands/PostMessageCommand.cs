
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Entities;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record PostMessageCommand(string ConversationId, string Text, string? ReplyToId) : IRequest<MessageDto>
{
    public class PostMessageCommandHandler : IRequestHandler<PostMessageCommand, MessageDto>
    {
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public PostMessageCommandHandler(IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            this.context = context;
            this._currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<MessageDto> Handle(PostMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await context.Conversations
                .Include(x => x.Participants)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.Id == request.ConversationId, cancellationToken);

            if (conversation is null) 
            {
                throw new Exception();
            }

            if(!conversation.Participants.Any(x => x.UserId == _currentUserService.UserId)) 
            {
                throw new Exception();
            }

            var message = new Message(request.Text, request.ReplyToId);

            conversation.AddMessage(message);

            await context.SaveChangesAsync(cancellationToken);

            message = await context.Messages
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .Include(c => c.DeletedBy)
                .Include(c => c.ReplyTo)
                .ThenInclude(c => c.CreatedBy)
                .ThenInclude(c => c.LastModifiedBy)
                .ThenInclude(c => c.DeletedBy)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.CreatedBy)
                //.Where(c => c.Item.Id == request.ItemId)
                .OrderByDescending(c => c.Created)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstAsync(x => x.Id == message.Id);

            var result = message.ToDto();

            await _bus.Publish(new MessagePosted(result));

            return message.ToDto();
        }
    }
}
