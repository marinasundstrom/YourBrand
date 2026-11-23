using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Models;

namespace YourBrand.Ticketing.Application.Features.Tickets.Queries;

public record GetTicketsGrouped(string OrganizationId, int? ProjectId, int[]? Status, string? AssigneeId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketGroupingDto>>
{
    public class Handler(ITicketRepository ticketRepository, IProjectRepository projectRepository, IDtoComposer dtoComposer) : IRequestHandler<GetTicketsGrouped, PagedResult<TicketGroupingDto>>
    {
        public async Task<PagedResult<TicketGroupingDto>> Handle(GetTicketsGrouped request, CancellationToken cancellationToken)
        {
            var query = ticketRepository.GetAll();

            if (request.ProjectId is not null)
            {
                query = query.Where(x => x.ProjectId == request.ProjectId.GetValueOrDefault());
            }

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status;
                query = query.Where(x => statuses.Any(z => x.StatusId == z));
            }

            /*

            if (request.AssigneeId is not null)
            {
                query = query.Where(x => x.AssigneeId == request.AssigneeId);
            }

            */

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var tickets = query
                .Include(i => i.Status)
                .Include(i => i.Type)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery();

            var x = await tickets.GroupBy(x => new { x.Assignee!.UserId, x.Assignee!.TeamMembershipId, x.Assignee.Name }).ToArrayAsync(cancellationToken);

            var totalCount = x.Length;

            var groups0 = x.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArray();

            List<TicketGroupingDto> r = new List<TicketGroupingDto>();

            foreach(var g in groups0)
            {
                r.Add(new TicketGroupingDto(g.Key!.Name!, g.Key.UserId, g.Key.TeamMembershipId, await dtoComposer.ComposeTicketDtos(g.ToArray(), cancellationToken)));
            }

            return new PagedResult<TicketGroupingDto>(r, totalCount);
        }
    }
}