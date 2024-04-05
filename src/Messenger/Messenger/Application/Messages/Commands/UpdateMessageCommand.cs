
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record UpdateMessageCommand(string ConversationId, string MessageId, string Text) : IRequest
{
    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand>
    {
        private readonly IMessengerContext context;
        private readonly IUserContext _userContext;
        private readonly IBus _bus;

        public UpdateMessageCommandHandler(IMessengerContext context, IUserContext userContext, IBus bus)
        {
            this.context = context;
            _userContext = userContext;
            _bus = bus;
        }

        public async Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await context.Messages.FirstOrDefaultAsync(i => i.Id == request.MessageId, cancellationToken);

            if (message is null) throw new Exception();

            if (!IsAuthorizedToEdit(message))
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }

            message.Text = request.Text;

            await context.SaveChangesAsync(cancellationToken);

            await _bus.Publish(new MessageUpdated(null!, message.Id, message.Text, DateTime.Now));

        }

        private bool IsAuthorizedToEdit(Domain.Entities.Message message) => _userContext.IsCurrentUser(message.CreatedById!) || _userContext.IsUserInRole(Roles.Administrator);
    }
}