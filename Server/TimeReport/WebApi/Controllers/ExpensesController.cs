
using Azure.Storage.Blobs;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Application.Expenses;
using TimeReport.Application.Expenses.Commands;
using TimeReport.Application.Expenses.Queries;

using static TimeReport.Application.Expenses.ExpensesHelpers;

namespace TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ExpenseDto>>> GetExpenses(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetExpensesQuery(page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> GetExpense(string id, CancellationToken cancellationToken)
    {
        var expense = await _mediator.Send(new GetExpenseQuery(id), cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(string projectId, CreateExpenseDto createExpenseDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new CreateExpenseCommand(projectId, createExpenseDto.Date, createExpenseDto.Amount, createExpenseDto.Description), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/Attachment")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadAttachment([FromRoute] string id, IFormFile file, CancellationToken cancellationToken)
    {
        var stream = file.OpenReadStream();

        var url = await _mediator.Send(new UploadExpenseAttachmentCommand(id, file.FileName, stream), cancellationToken);

        return Ok(url);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(string id, UpdateExpenseDto updateExpenseDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new UpdateExpenseCommand(id, updateExpenseDto.Date, updateExpenseDto.Amount, updateExpenseDto.Description), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteExpense(string id, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new DeleteExpenseCommand(id), cancellationToken);

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

public record class CreateExpenseDto(DateTime Date, decimal Amount, string? Description);

public record class UpdateExpenseDto(DateTime Date, decimal Amount, string? Description);