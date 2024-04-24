using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Stores.Commands;

public sealed record CreateStoreCommand(string Name, string Handle, string Currency) : IRequest<StoreDto>
{
    public sealed class CreateStoreCommandHandler(CatalogContext context) : IRequestHandler<CreateStoreCommand, StoreDto>
    {
        public async Task<StoreDto> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            var store = await context.Stores.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (store is not null) throw new Exception();

            var currency = await context.Currencies.FirstAsync(i => i.Code == request.Currency, cancellationToken);

            store = new Catalog.Domain.Entities.Store(request.Name, request.Handle, currency);
            store.CurrencyDisplayOptions = new Domain.Entities.CurrencyDisplayOptions() {
                IncludeVatInSalesPrice = true
            };

            context.Stores.Add(store);

            await context.SaveChangesAsync(cancellationToken);

            store = await context
               .Stores
               .Include(x => x.Currency)
               .AsNoTracking()
               .FirstAsync(c => c.Id == store.Id);

            return store.ToDto();
        }
    }
}