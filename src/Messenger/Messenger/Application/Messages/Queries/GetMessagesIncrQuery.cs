
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Messages.Queries;

public record GetMessagesIncrQuery(
    string ConversationId, int Skip, int Take, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null)
    : IRequest<Results<MessageDto>>
{
    public class GetMessagesIncrQueryHandler : IRequestHandler<GetMessagesIncrQuery, Results<MessageDto>>
    {
        private readonly IMessengerContext context;

        public GetMessagesIncrQueryHandler(IMessengerContext context)
        {
            this.context = context;
        }

        public async Task<Results<MessageDto>> Handle(GetMessagesIncrQuery request, CancellationToken cancellationToken)
        {
            var query = context.Messages
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .Include(c => c.DeletedBy)
                .Include(c => c.ReplyTo)
                .ThenInclude(c => c.CreatedBy)
                .ThenInclude(c => c.LastModifiedBy)
                .ThenInclude(c => c.DeletedBy)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.CreatedBy)
                .Where(c => c.ConversationId == request.ConversationId)
                .OrderByDescending(c => c.Created)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .AsNoTracking()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            query = query
                .Skip(request.Skip)
                .Take(request.Take).AsQueryable();

            var messages = await query
                .ToListAsync(cancellationToken);

            return new Results<MessageDto>(
                messages.Select(message => message.ToDto()),
                totalCount);
        }
    }
}