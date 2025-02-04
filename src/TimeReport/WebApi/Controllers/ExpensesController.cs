﻿using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects.Expenses;
using YourBrand.TimeReport.Application.Projects.Expenses.Commands;
using YourBrand.TimeReport.Application.Projects.Expenses.Queries;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ExpensesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ExpenseDto>>> GetExpenses(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetExpensesQuery(organizationId, page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> GetExpense(string organizationId, string id, CancellationToken cancellationToken)
    {
        var expense = await mediator.Send(new GetExpenseQuery(organizationId, id), cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(string organizationId, string projectId, CreateExpenseDto createExpenseDto, CancellationToken cancellationToken)
    {
        try
        {
            var task = await mediator.Send(new CreateExpenseCommand(organizationId, projectId, createExpenseDto.Date, createExpenseDto.ExpenseTypeId, createExpenseDto.Amount, createExpenseDto.Description), cancellationToken);

            return Ok(task);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/Attachment")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadAttachment(string organizationId, [FromRoute] string id, IFormFile file, CancellationToken cancellationToken)
    {
        var stream = file.OpenReadStream();

        var url = await mediator.Send(new UploadExpenseAttachmentCommand(organizationId, id, file.FileName, stream), cancellationToken);

        return Ok(url);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(string organizationId, string id, UpdateExpenseDto updateExpenseDto, CancellationToken cancellationToken)
    {
        try
        {
            var task = await mediator.Send(new UpdateExpenseCommand(organizationId, id, updateExpenseDto.Date, updateExpenseDto.Amount, updateExpenseDto.Description), cancellationToken);

            return Ok(task);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteExpense(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteExpenseCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    /*
    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string id)
    {
        var expense = await context.Expenses
            .Include(x => x.Entries)
            .ThenInclude(x => x.User)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (expense is null)
        {
            return NotFound();
        }

        var totalHours = expense.Entries
            .Sum(p => p.Hours.GetValueOrDefault());

        var totalUsers = expense.Entries
            .Select(p => p.User)
            .DistinctBy(p => p.Id)
            .Count();

        return new StatisticsSummary(new StatisticsSummaryEntry[]
        {
            new ("Participants", totalUsers),
            new ("Hours", totalHours)
        });
    }
    */
}

public record class CreateExpenseDto(DateTime Date, string ExpenseTypeId, decimal Amount, string? Description);

public record class UpdateExpenseDto(DateTime Date, decimal Amount, string? Description);