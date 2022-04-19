
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Queries;

public record GetCommentsQuery(string ItemId, int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<CommentDto>>
{
    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Results<CommentDto>>
    {
        private readonly ICatalogContext context;

        public GetCommentsQueryHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task<Results<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.LastModifiedBy)
                .Where(c => c.Item.Id == request.ItemId)
                .OrderByDescending(c => c.Created)
                .AsSplitQuery()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            query = query.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            var comments = await query.ToListAsync(cancellationToken);

            return new Results<CommentDto>(
                comments.Select(comment => new CommentDto(comment.Id, comment.Text, comment.Created, comment.CreatedBy?.ToDto(), comment.LastModified, comment.LastModifiedBy?.ToDto())),
                totalCount);
        }
    }
}