
using Catalog.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.Commands;

public class PostCommentCommand : IRequest
{
    public string ItemId { get; set; } = null!;

    public string Text { get; set; } = null!;

    public PostCommentCommand(string itemId, string text)
    {
        ItemId = itemId;
        Text = text;
    }

    public class PostCommentCommandHandler : IRequestHandler<PostCommentCommand>
    {
        private readonly ICatalogContext context;

        public PostCommentCommandHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(PostCommentCommand request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item is null) throw new Exception();

            item.AddComment(request.Text);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}