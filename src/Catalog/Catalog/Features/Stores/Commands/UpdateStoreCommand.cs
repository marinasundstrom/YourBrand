using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Stores.Commands;

public sealed record UpdateStoreCommand(string OrganizationId, string Id, string Name, string Handle, string Currency) : IRequest
{
    public sealed class UpdateStoreCommandHandler(CatalogContext context) : IRequestHandler<UpdateStoreCommand>
    {
        public async Task Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await context.Stores
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (store is null) throw new Exception();

            store.Name = request.Name;
            store.Handle = request.Handle;
            //store.Currency = request.Currency;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}