
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Identity;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record UpdateMessageCommand(string MessageId, string Text) : IRequest
{
    public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand>
    {
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public UpdateMessageCommandHandler(IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            this.context = context;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<Unit> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
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

            return Unit.Value;
        }

        private bool IsAuthorizedToEdit(Domain.Entities.Message message) => _currentUserService.IsCurrentUser(message.CreatedById!) || _currentUserService.IsUserInRole(Roles.Administrator);
    }
}
