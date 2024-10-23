using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Users;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;
using YourBrand.Extensions;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public record GetMessages(OrganizationId OrganizationId, ChannelId? ChannelId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<MessageDto>>
{
    public class Handler(ApplicationDbContext context, IDtoComposer dtoComposer) : IRequestHandler<GetMessages, ItemsResult<MessageDto>>
    {
        public async Task<ItemsResult<MessageDto>> Handle(GetMessages request, CancellationToken cancellationToken)
        {
            var query = context.Messages
                .InOrganization(request.OrganizationId)
                .IgnoreQueryFilters()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.ChannelId is not null)
            {
                query = query.Where(x => x.ChannelId == request.ChannelId);
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