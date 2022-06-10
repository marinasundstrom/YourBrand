
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Messages.Commands;

public record SendMessageReceiptCommand(string MessageId) : IRequest<ReceiptDto>
{
    public class SendMessageReceiptCommandHandler : IRequestHandler<SendMessageReceiptCommand, ReceiptDto>
    {
        private readonly IMessengerContext context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBus _bus;

        public SendMessageReceiptCommandHandler(IMessengerContext context, ICurrentUserService currentUserService, IBus bus)
        {
            this.context = context;
            _currentUserService = currentUserService;
            _bus = bus;
        }

        public async Task<ReceiptDto> Handle(SendMessageReceiptCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var message = await context
                .Messages
                .Include(x => x.Receipts)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.Id == request.MessageId, cancellationToken);

            if (message is null) throw new Exception();

            var receipt = message.Receipts.FirstOrDefault(x => x.CreatedById == userId);

            if (receipt is not null)
            {
                return receipt.ToDto();
            }

            receipt = new Domain.Entities.MessageReceipt()
            {
                Id = Guid.NewGuid().ToString()
            };

            message.AddReceipt(receipt);

            await context.SaveChangesAsync(cancellationToken);

            receipt = await context
                .MessageReceipts
                .Include(x => x.CreatedBy)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstAsync(i => i.Id == receipt.Id, cancellationToken);

            var dto = receipt.ToDto();

            await _bus.Publish(new MessageRead(dto));

            return dto;
        }
    }
}
