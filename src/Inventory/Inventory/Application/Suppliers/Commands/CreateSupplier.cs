using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Suppliers.Commands;

public record CreateSupplier(string Id, string Name) : IRequest<SupplierDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<CreateSupplier, SupplierDto>
    {
        public async Task<SupplierDto> Handle(CreateSupplier request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new ArgumentException("Supplier id cannot be empty.", nameof(request.Id));
            }

            if (await context.Suppliers.AnyAsync(x => x.Id == request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Supplier with id '{request.Id}' already exists.");
            }

            var supplier = new Supplier(request.Id, request.Name);

            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync(cancellationToken);

            return supplier.ToDto();
        }
    }
}
