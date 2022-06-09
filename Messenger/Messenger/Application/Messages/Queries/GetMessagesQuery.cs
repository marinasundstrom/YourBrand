
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Contracts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Messages.Queries;

public record GetMessagesQuery(
    string ItemId, int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null)
    : IRequest<Results<MessageDto>>
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Results<MessageDto>>
    {
        private readonly IMessengerContext context;

        public GetMessagesQueryHandler(IMessengerContext context)
        {
            this.context = context;
        }

        public async Task<Results<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var query = context.Messages
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                //.Where(c => c.Item.Id == request.ItemId)
                .OrderByDescending(c => c.Created)
                .AsNoTracking()
                .AsSplitQuery()
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

            var messages = await query.ToListAsync(cancellationToken);

            return new Results<MessageDto>(
                messages.Select(message => message.ToDto()),
                totalCount);
        }
    }
}