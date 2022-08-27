using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Documents.Application;
using YourBrand.Documents.Application.Commands;
using YourBrand.Documents.Contracts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Documents.Application.Queries;
using YourBrand.Documents.Application.Common.Models;

namespace YourBrand.Documents.Controllers;

[Route("[controller]")]
public class DocumentsController : Controller
{
    [HttpGet]
    public async Task<ItemsResult<DocumentDto>> GetDocuments(int page, int pageSize, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetDocuments(page, pageSize), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<DocumentDto?> GetDocument(string id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetDocument(id), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteDocument(string id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteDocument(id), cancellationToken);
    }

    [HttpGet("{id}/File")]
    public async Task<IActionResult> GetFile(string id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        DocumentFileResponse? response = await mediator.Send(new GetDocumentFile(id), cancellationToken);
        if (response is null) return NotFound();
        return File(response.Stream, response.ContentType, $"{response.FileName}");
    }

    [HttpPut("{id}/Name")]
    public async Task RenameDocument(string id, string newName, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new RenameDocument(id, newName), cancellationToken);
    }

    [HttpPut("{id}/CanRename")]
    public async Task<bool> CanRenameDocument(string id, string newName, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CanRenameDocument(id, newName), cancellationToken);
    }

    [HttpGet("{id}/IsNameTaken")]
    public async Task<bool> CheckNameTaken(string id, string name, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CheckNameTaken(name), cancellationToken);
    }

    [HttpPut("{id}/Description")]
    public async Task UpdateDescription(string id, string description, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateDescription(id, description), cancellationToken);
    }

    [HttpPost("UploadDocument")]
    public async Task<DocumentDto> UploadDocument(string directoryId, IFormFile file, [FromServices] IMediator mediator, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new UploadDocument(file.FileName, file.ContentType, file.OpenReadStream(), directoryId), cancellationToken);
    }

    [HttpPost("GenerateDocument")]
    public async Task<IActionResult> GenerateDocument(string templateId, DocumentFormat documentFormat, [FromBody] string model, [FromServices] IMediator mediator, CancellationToken cancellationToken = default)
    {
        //DocumentFormat documentFormat = DocumentFormat.Html;

        var stream = await mediator.Send(new GenerateDocument(templateId, documentFormat, model), cancellationToken);
        return File(stream, documentFormat == DocumentFormat.Html ? "application/html" : "application/pdf");
    }
}