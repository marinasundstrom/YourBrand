
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Queries;

public record GetCommentQuery(string Id) : IRequest<CommentDto?>
{
    public class GetCommentQueryHandler : IRequestHandler<GetCommentQuery, CommentDto?>
    {
        private readonly IAppServiceContext context;

        public GetCommentQueryHandler(IAppServiceContext context)
        {
            this.context = context;
        }

        public async Task<CommentDto?> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            var comment = await context.Comments
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (comment is null) return null;

            return comment.ToDto();
        }
    }
}