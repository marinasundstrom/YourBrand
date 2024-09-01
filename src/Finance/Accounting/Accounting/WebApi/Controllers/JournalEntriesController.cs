using Asp.Versioning;

using Azure.Storage.Blobs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Application.Journal;
using YourBrand.Accounting.Application.Journal.Commands;
using YourBrand.Accounting.Application.Journal.Queries;

namespace YourBrand.Accounting.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class JournalEntriesController(IMediator mediator) : Controller
{

    // GET: api/values
    [HttpGet]
    public async Task<JournalEntryResult> GetJournalEntriesAsync(string organizationId, int page = 0, int pageSize = 10, int? invoiceNo = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetJournalEntriesQuery(organizationId, page, pageSize, invoiceNo), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JournalEntryDto>> GetJournalEntryAsync(string organizationId, int id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetJournalEntryQuery(organizationId, id), cancellationToken);
    }

    [HttpPost]
    public async Task<int> CreateJournalEntry(string organizationId, [FromBody] CreateJournalEntry dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateJournalEntryCommand(organizationId, dto.Description, dto.InvoiceNo, dto.Entries), cancellationToken);
    }

    [HttpPost("{id}/Verifications")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string?>> AddFileToJournalEntryAsVerification(string organizationId, int id, string? description, int? invoiceId, IFormFile file, CancellationToken cancellationToken)
    {
        return await mediator.Send(new AddFileAsVerificationCommand(organizationId, id, file.FileName, file.ContentType, description, invoiceId, file.OpenReadStream()), cancellationToken);
    }
}