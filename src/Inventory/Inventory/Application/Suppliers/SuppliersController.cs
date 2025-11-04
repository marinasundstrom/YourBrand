using System;
using System.Collections.Generic;
using System.Linq;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Inventory.Application.Suppliers.Commands;
using YourBrand.Inventory.Application.Suppliers.Queries;

namespace YourBrand.Inventory.Application.Suppliers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Suppliers")]
public class SuppliersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<SupplierDto>> GetSuppliers(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSuppliers(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<SupplierDto?> GetSupplier(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSupplier(id), cancellationToken);
    }

    [HttpPost]
    public async Task<SupplierDto> CreateSupplier(CreateSupplierDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateSupplier(dto.Id, dto.Name), cancellationToken);
    }

    [HttpPost("{supplierId}/Items")]
    public async Task<SupplierItemDto> AddSupplierItem(string supplierId, AddSupplierItemDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new AddSupplierItem(supplierId, dto.ItemId, dto.SupplierItemId, dto.UnitCost, dto.LeadTimeDays), cancellationToken);
    }

    [HttpGet("{supplierId}/Orders")]
    public async Task<IEnumerable<SupplierOrderDto>> GetSupplierOrders(string supplierId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSupplierOrders(supplierId), cancellationToken);
    }

    [HttpGet("{supplierId}/Orders/{orderId}")]
    public async Task<SupplierOrderDto?> GetSupplierOrder(string supplierId, string orderId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSupplierOrder(supplierId, orderId), cancellationToken);
    }

    [HttpPost("{supplierId}/Orders")]
    public async Task<SupplierOrderDto> CreateSupplierOrder(string supplierId, CreateSupplierOrderDto dto, CancellationToken cancellationToken)
    {
        var lines = dto.Lines.Select(line => new CreateSupplierOrderLine(line.SupplierItemId, line.ExpectedQuantity, line.ExpectedOn)).ToList();

        return await mediator.Send(new CreateSupplierOrder(supplierId, dto.OrderNumber, dto.OrderedAt, dto.ExpectedDelivery, lines), cancellationToken);
    }
}

public record CreateSupplierDto(string Id, string Name);

public record AddSupplierItemDto(string ItemId, string? SupplierItemId, decimal? UnitCost, int? LeadTimeDays);

public record CreateSupplierOrderDto(string OrderNumber, DateTime OrderedAt, DateTime? ExpectedDelivery, IReadOnlyCollection<CreateSupplierOrderLineDto> Lines);

public record CreateSupplierOrderLineDto(string SupplierItemId, int ExpectedQuantity, DateTime? ExpectedOn);
