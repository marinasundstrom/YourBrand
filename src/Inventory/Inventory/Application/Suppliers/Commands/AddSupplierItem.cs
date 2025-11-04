using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Suppliers.Commands;

public record AddSupplierItem(
    string SupplierId,
    string ItemId,
    string? SupplierItemId,
    decimal? UnitCost,
    int? LeadTimeDays) : IRequest<SupplierItemDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<AddSupplierItem, SupplierItemDto>
    {
        public async Task<SupplierItemDto> Handle(AddSupplierItem request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers
                .Include(x => x.Items)
                    .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == request.SupplierId, cancellationToken);

            if (supplier is null)
            {
                throw new InvalidOperationException($"Supplier '{request.SupplierId}' was not found.");
            }

            var item = await context.Items.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);

            if (item is null)
            {
                throw new InvalidOperationException($"Item '{request.ItemId}' was not found.");
            }

            var supplierItem = new SupplierItem(supplier, item, request.SupplierItemId, request.UnitCost, request.LeadTimeDays);

            context.SupplierItems.Add(supplierItem);

            await context.SaveChangesAsync(cancellationToken);

            return supplierItem.ToDto();
        }
    }
}
