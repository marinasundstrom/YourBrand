using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Users;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;
using YourBrand.Extensions;

using Errors = YourBrand.ChatApp.Domain.Errors.Channels;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public record GetChannels(OrganizationId OrganizationId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<ChannelDto>>
{
    public class Handler(ApplicationDbContext context) : IRequestHandler<GetChannels, ItemsResult<ChannelDto>>
    {
        public async Task<ItemsResult<ChannelDto>> Handle(GetChannels request, CancellationToken cancellationToken)
        {
            var query = context.Channels
                .InOrganization(request.OrganizationId)
                .AsQueryable();

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

public record GetChannelById(OrganizationId OrganizationId, ChannelId Id) : IRequest<Result<ChannelDto>>
{
    public class Validator : AbstractValidator<GetChannelById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(ApplicationDbContext applicationDbContext) : IRequestHandler<GetChannelById, Result<ChannelDto>>
    {
        public async Task<Result<ChannelDto>> Handle(GetChannelById request, CancellationToken cancellationToken)
        {
            var channel = await applicationDbContext
                .Channels
                //.Include(x => x.Participants)
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (channel is null)
            {
                return Errors.ChannelNotFound;
            }

            return channel.ToDto();
        }
    }
}