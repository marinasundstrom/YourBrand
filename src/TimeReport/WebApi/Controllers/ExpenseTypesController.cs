using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes;
using YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;
using YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Queries;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class ExpenseTypesController(IMediator mediator, ITimeReportContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ExpenseTypeDto>>> GetExpenseTypes(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetExpenseTypesQuery(page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseTypeDto>> GetExpenseType(string id, CancellationToken cancellationToken)
    {
        var expense = await mediator.Send(new GetExpenseTypeQuery(id), cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseTypeDto>> CreateExpenseType(CreateExpenseTypeDto createExpenseTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new CreateExpenseTypeCommand(createExpenseTypeDto.Name, createExpenseTypeDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseTypeDto>> UpdateExpenseType(string id, UpdateExpenseTypeDto updateExpenseTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new UpdateExpenseTypeCommand(id, updateExpenseTypeDto.Name, updateExpenseTypeDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteExpenseType(string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteExpenseTypeCommand(id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateExpenseTypeDto(string Name, string? Description);

public record class UpdateExpenseTypeDto(string Name, string? Description);