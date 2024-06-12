using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Common;
using ChatApp.Extensions;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Features.Users;

namespace ChatApp.Features.Chat.Messages;

public record GetMessages(Guid? ChannelId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<MessageDto>>
{
    public class Handler : IRequestHandler<GetMessages, ItemsResult<MessageDto>>
    {
        private readonly IMessageRepository messageRepository;
        private readonly ApplicationDbContext context;
        private readonly IDtoComposer dtoComposer;

        public Handler(IMessageRepository messageRepository, ApplicationDbContext context, IDtoComposer dtoComposer)
        {
            this.messageRepository = messageRepository;
            this.context = context;
            this.dtoComposer = dtoComposer;
        }

        public async Task<ItemsResult<MessageDto>> Handle(GetMessages request, CancellationToken cancellationToken)
        {
            var query = context.Messages
                .IgnoreQueryFilters()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if(request.ChannelId is not null) 
            {
                query = query.Where(x => x.ChannelId == request.ChannelId);
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
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
