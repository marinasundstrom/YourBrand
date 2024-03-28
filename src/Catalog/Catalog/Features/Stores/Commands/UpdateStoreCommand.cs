using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.Stores.Commands;

public sealed record UpdateStoreCommand(string Id, string Name, string Handle, string Currency) : IRequest
{
    public sealed class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand>
    {
        private readonly CatalogContext context;

        public UpdateStoreCommandHandler(CatalogContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await context.Stores.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (store is null) throw new Exception();

            store.Name = request.Name;
            store.Handle = request.Handle;
            //store.Currency = request.Currency;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}