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
public class VerificationsController(IMediator mediator) : Controller
{

    // GET: api/values
    [HttpGet]
    public async Task<VerificationResult> GetVerificationsAsync(string organizationId, int page = 0, int pageSize = 10, int? invoiceNo = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetVerificationsQuery(organizationId, page, pageSize, invoiceNo), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VerificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VerificationDto>> GetVerificationAsync(string organizationId, int id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetVerificationQuery(organizationId, id), cancellationToken);
    }

    [HttpPost]
    public async Task<int> CreateVerification(string organizationId, [FromBody] CreateVerification dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateVerificationCommand(organizationId, dto.Description, dto.InvoiceNo, dto.Entries), cancellationToken);
    }

    [HttpPost("{id}/SupportingDocuments")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string?>> AddFileToVerificationAsSupportingDocument(string organizationId, int id, string? description, int? invoiceId, IFormFile file, CancellationToken cancellationToken)
    {
        return await mediator.Send(new AddFileAsSupportingDocumentCommand(organizationId, id, file.FileName, file.ContentType, description, invoiceId, file.OpenReadStream()), cancellationToken);
    }
}