using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Features.Tasks.TaskCharges;
using YourBrand.TimeReport.Features.Tasks.TaskCharges.Commands;
using YourBrand.TimeReport.Features.Tasks.TaskCharges.Queries;
using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Features.Tasks;

partial class TasksController
{
    [HttpGet("{taskId}/Charges")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TaskChargeDto>>> GetTaskCharges(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? taskId = null, string? searchString = null, string? sortBy = null, TimeReport.Features.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetTaskChargesQuery(organizationId, page, pageSize, projectId, taskId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{taskId}/Charges/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskChargeDto>> GetTaskCharge(string organizationId, string taskId, string id, CancellationToken cancellationToken)
    {
        var charge = await mediator.Send(new GetTaskChargeQuery(organizationId, id), cancellationToken);

        if (charge is null)
        {
            return NotFound();
        }

        return Ok(charge);
    }

    [HttpPost("{taskId}/Charges")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskChargeDto>> CreateTaskCharge(string organizationId, string taskId, CreateTaskChargeDto createTaskChargeDto, CancellationToken cancellationToken)
    {
        try
        {
            var charge = await mediator.Send(new CreateTaskChargeCommand(organizationId, createTaskChargeDto.Description, null, taskId, createTaskChargeDto.Type, createTaskChargeDto.Basis, createTaskChargeDto.Amount), cancellationToken);

            return Ok(charge);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{taskId}/Charges/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskChargeDto>> UpdateTaskCharge(string organizationId, string taskId, string id, UpdateTaskChargeDto updateTaskChargeDto, CancellationToken cancellationToken)
    {
        try
        {
            var charge = await mediator.Send(new UpdateTaskChargeCommand(organizationId, id, updateTaskChargeDto.Description, null, taskId, updateTaskChargeDto.Type, updateTaskChargeDto.Basis, updateTaskChargeDto.Amount), cancellationToken);

            return Ok(charge);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{taskId}/Charges/{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteTaskCharge(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteTaskChargeCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateTaskChargeDto(
    ChargeType Type,
    ChargeBasis Basis,
    decimal Amount,
    string? Description);

public record class UpdateTaskChargeDto(
    ChargeType Type,
    ChargeBasis Basis,
    decimal Amount,
    string? Description);