using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Stores.Commands;

public sealed record DeleteStoreCommand(string Id) : IRequest
{
    public sealed class DeleteStoreCommandHandler(CatalogContext context) : IRequestHandler<DeleteStoreCommand>
    {
        public async Task Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await context.Stores
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (store is null) throw new Exception();

            context.Stores.Remove(store);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}