
using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Warehouses.Items;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record CreateWarehouseItemReservation(string WarehouseId, string Id, int Quantity, int? HoldDurationMinutes = null, string? Reference = null) : IRequest<WarehouseItemReservationDto>
{
    private const int DefaultHoldDurationMinutes = 15;

    public class Handler(IInventoryContext context) : IRequestHandler<CreateWarehouseItemReservation, WarehouseItemReservationDto>
    {
        public async Task<WarehouseItemReservationDto> Handle(CreateWarehouseItemReservation request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Quantity));
            }

            if (request.HoldDurationMinutes.HasValue && request.HoldDurationMinutes.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.HoldDurationMinutes));
            }

            var item = await context.WarehouseItems
                .Include(i => i.Reservations)
                .FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null)
            {
                throw new Exception();
            }

            var duration = TimeSpan.FromMinutes(request.HoldDurationMinutes ?? DefaultHoldDurationMinutes);

            var reservation = item.Reserve(request.Quantity, duration, request.Reference);

            await context.SaveChangesAsync(cancellationToken);

            return reservation.ToDto();
        }
    }
}