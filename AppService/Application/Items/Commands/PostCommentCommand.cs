
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Commands;

public record PostCommentCommand(string ItemId, string Text) : IRequest
{
    public class PostCommentCommandHandler : IRequestHandler<PostCommentCommand>
    {
        private readonly ICatalogContext context;

        public PostCommentCommandHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task Handle(PostCommentCommand request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item is null) throw new Exception();

            item.AddComment(request.Text);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
