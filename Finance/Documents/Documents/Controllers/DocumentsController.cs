using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Documents.Application;
using Documents.Application.Commands;
using Documents.Contracts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Documents.Controllers;

[Route("[controller]")]
public class DocumentsController : Controller
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("UploadDocument")]
    public async Task<DocumentDto> UploadDocument(IFormFile file, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new UploadDocument(file.FileName, file.ContentType, file.OpenReadStream()), cancellationToken);
    }

    [HttpPost("GenerateDocument")]
    public async Task<IActionResult> GenerateDocument(string templateId, DocumentFormat documentFormat, [FromBody] string model, CancellationToken cancellationToken = default)
    {
        //DocumentFormat documentFormat = DocumentFormat.Html;

        var stream = await _mediator.Send(new GenerateDocument(templateId, documentFormat, model), cancellationToken);
        return File(stream, documentFormat == DocumentFormat.Html ? "application/html" : "application/pdf");
    }
}