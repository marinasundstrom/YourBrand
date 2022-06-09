
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Conversations.Queries;

public record GetConversationsQuery(
    int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null)
    : IRequest<Results<ConversationDto>>
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, Results<ConversationDto>>
    {
        private readonly IMessengerContext context;

        public GetConversationsQueryHandler(IMessengerContext context)
        {
            this.context = context;
        }

        public async Task<Results<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .OrderByDescending(c => c.Created)
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

            query = query.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            var conversations = await query.ToListAsync(cancellationToken);

            return new Results<ConversationDto>(
                conversations.Select(message => message.ToDto()),
                totalCount);
        }
    }
}