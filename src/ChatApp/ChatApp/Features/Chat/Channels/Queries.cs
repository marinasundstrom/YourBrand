using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain;
using YourBrand.Extensions;
using YourBrand.ChatApp.Features.Users;
using YourBrand.ChatApp.Infrastructure.Persistence;

using Errors = YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public record GetChannels(int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<ChannelDto>>
{
    public class Handler(IChannelRepository channelRepository, ApplicationDbContext context) : IRequestHandler<GetChannels, ItemsResult<ChannelDto>>
    {
        public async Task<ItemsResult<ChannelDto>> Handle(GetChannels request, CancellationToken cancellationToken)
        {
            var query = context.Channels.AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderBy(x => x.Created);
            }

            var channels = await query
                .AsNoTracking()
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<ChannelDto>(channels.Select(x => x.ToDto()), totalCount);
        }
    }
}

public record GetChannelById(Guid Id) : IRequest<Result<ChannelDto>>
{
    public class Validator : AbstractValidator<GetChannelById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(IChannelRepository channelRepository) : IRequestHandler<GetChannelById, Result<ChannelDto>>
    {
        public async Task<Result<ChannelDto>> Handle(GetChannelById request, CancellationToken cancellationToken)
        {
            var todo = await channelRepository.FindByIdAsync(request.Id, cancellationToken);

            if (todo is null)
            {
                return Errors.ChannelNotFound;
            }

            return todo.ToDto();
        }
    }
}