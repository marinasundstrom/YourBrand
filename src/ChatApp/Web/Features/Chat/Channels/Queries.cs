using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Features.Users;

namespace ChatApp.Features.Chat.Channels;

public record GetChannels(int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<ChannelDto>>
{
    public class Handler : IRequestHandler<GetChannels, ItemsResult<ChannelDto>>
    {
        private readonly IChannelRepository channelRepository;
        private readonly ApplicationDbContext context;

        public Handler(IChannelRepository channelRepository, ApplicationDbContext context)
        {
            this.channelRepository = channelRepository;
            this.context = context;
        }

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

    public class Handler : IRequestHandler<GetChannelById, Result<ChannelDto>>
    {
        private readonly IChannelRepository channelRepository;

        public Handler(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public async Task<Result<ChannelDto>> Handle(GetChannelById request, CancellationToken cancellationToken)
        {
            var todo = await channelRepository.FindByIdAsync(request.Id, cancellationToken);

            if (todo is null)
            {
                return Result.Failure<ChannelDto>(Errors.Channels.ChannelNotFound);
            }

            return Result.Success(todo.ToDto());
        }
    }
}