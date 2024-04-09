
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record UpdateMessageCommand(string ConversationId, string MessageId, string Text) : IRequest
{
    public class UpdateMessageCommandHandler(IMessengerContext context, IUserContext userContext, IBus bus) : IRequestHandler<UpdateMessageCommand>
    {
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

            await bus.Publish(new MessageUpdated(null!, message.Id, message.Text, DateTime.Now));

        }

        private bool IsAuthorizedToEdit(Domain.Entities.Message message) => userContext.IsCurrentUser(message.CreatedById!) || userContext.IsUserInRole(Roles.Administrator);
    }
}