﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Users;

public record GetUsers(int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<UserDto>>
{
    public class Handler(IUserRepository userRepository) : IRequestHandler<GetUsers, PagedResult<UserDto>>
    {
        public async Task<PagedResult<UserDto>> Handle(GetUsers request, CancellationToken cancellationToken)
        {
            var query = userRepository.GetAll();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SearchTerm is not null)
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var users = await query
                .OrderBy(i => i.Id)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<UserDto>(users.Select(x => x.ToDto()), totalCount);
        }
    }
}