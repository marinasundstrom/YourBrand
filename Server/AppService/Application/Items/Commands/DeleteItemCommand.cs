
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.Commands;

public class DeleteItemCommand : IRequest<DeletionResult>
{
    public string Id { get; set; }

    public DeleteItemCommand(string id)
    {
        Id = id;
    }

    public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, DeletionResult>
    {
        private readonly ICatalogContext context;
        private readonly IItemsClient client;

        public DeleteItemCommandHandler(ICatalogContext context, IItemsClient client)
        {
            this.context = context;
            this.client = client;
        }

        public async Task<DeletionResult> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                return DeletionResult.NotFound;
            }

            item.DomainEvents.Add(new ItemDeletedEvent(item.Id, item.Name));

            context.Items.Remove(item);

            await context.SaveChangesAsync(cancellationToken);

            return DeletionResult.Successful;
        }
    }
}