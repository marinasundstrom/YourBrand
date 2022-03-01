
using Skynet.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Skynet.Application.Items.Commands;

public record UpdateCommentCommand(string CommentId, string Text) : IRequest
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand>
    {
        private readonly ICatalogContext context;

        public UpdateCommentCommandHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await context.Comments.FirstOrDefaultAsync(i => i.Id == request.CommentId, cancellationToken);

            if (comment is null) throw new Exception();

            comment.Text = request.Text;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
