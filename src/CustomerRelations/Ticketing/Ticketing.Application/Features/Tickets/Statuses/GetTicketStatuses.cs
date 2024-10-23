﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Models;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses;

public record GetTicketStatuses(string? SearchTerm, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketStatusDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetTicketStatuses, PagedResult<TicketStatusDto>>
    {
        public async Task<PagedResult<TicketStatusDto>> Handle(GetTicketStatuses request, CancellationToken cancellationToken)
        {
            var query = context.TicketStatuses.AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            /*
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }
            */

            var statuses = await query
                /*
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy) */
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<TicketStatusDto>(statuses.Select(x => x.ToDto()), totalCount);
        }
    }
}