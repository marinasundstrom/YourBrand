
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Messages.Queries;

public record GetMessageQuery(string Id) : IRequest<MessageDto?>
{
    public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, MessageDto?>
    {
        private readonly IMessengerContext context;

        public GetMessageQueryHandler(IMessengerContext context)
        {
            this.context = context;
        }

        public async Task<MessageDto?> Handle(GetMessageQuery request, CancellationToken cancellationToken)
        {
            var message = await context.Messages
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (message is null) return null;

            return message.ToDto();
        }
    }
}