
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Commands;

public record DeleteCommentCommand(string CommentId) : IRequest
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly ICatalogContext context;

        public DeleteCommentCommandHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await context.Comments
                .Include(x => x.Item)
                .FirstOrDefaultAsync(i => i.Id == request.CommentId, cancellationToken);

            if (comment is null) throw new Exception();

            context.Comments.Remove(comment);

            comment.Item.CommentCount--;
           
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}