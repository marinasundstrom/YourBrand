using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Extensions;
using YourBrand.ChatApp.Features.Users;
using YourBrand.ChatApp.Infrastructure.Persistence;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public record GetMessages(Guid? ChannelId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<MessageDto>>
{
    public class Handler(IMessageRepository messageRepository, ApplicationDbContext context, IDtoComposer dtoComposer) : IRequestHandler<GetMessages, ItemsResult<MessageDto>>
    {
        public async Task<ItemsResult<MessageDto>> Handle(GetMessages request, CancellationToken cancellationToken)
        {
            var query = context.Messages
                .IgnoreQueryFilters()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.ChannelId is not null)
            {
                var cid = new ChannelId(request.ChannelId.GetValueOrDefault());
                query = query.Where(x => x.ChannelId == cid);
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Posted);
            }

            var messages = await query
                .AsNoTracking()
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            IEnumerable<MessageDto> dtos = await dtoComposer.ComposeMessageDtos(messages, cancellationToken);

            return new ItemsResult<MessageDto>(dtos!, totalCount);
        }
    }
}