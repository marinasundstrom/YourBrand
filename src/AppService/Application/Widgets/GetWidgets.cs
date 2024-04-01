using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;

namespace YourBrand.Application.Widgets;

public record GetWidgets(int Page = 1, int PageSize = 10, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<WidgetDto>>
{
    public class Handler : IRequestHandler<GetWidgets, Results<WidgetDto>>
    {
        private readonly IAppServiceContext context;

        public Handler(IAppServiceContext context)
        {
            this.context = context;
        }

        public async Task<Results<WidgetDto>> Handle(GetWidgets request, CancellationToken cancellationToken)
        {
            var query = context.Widgets.AsQueryable();

            /*
            if (request.Status is not null)
            {
                query = query.Where(x => x.Status == (WidgetStatus)request.Status);
            }

            if (request.AssigneeId is not null)
            {
                query = query.Where(x => x.AssigneeId == request.AssigneeId);
            }
            */

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? YourBrand.Application.SortDirection.Descending : YourBrand.Application.SortDirection.Ascending);
            }
            /*
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }*/

            var widgets = await query
                //.Include(i => i.Assignee)
                //.Include(i => i.CreatedBy)
                //.Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new Results<WidgetDto>(widgets.Select(x => x.ToDto()), totalCount);
        }
    }
}