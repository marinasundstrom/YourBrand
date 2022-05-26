using Accounting.Application.Common.Interfaces;
using Accounting.Application.Verifications;
using Accounting.Application.Verifications.Commands;
using Accounting.Application.Verifications.Queries;

using Azure.Storage.Blobs;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [Route("[controller]")]
    public class VerificationsController : Controller
    {
        private readonly IMediator mediator;
        private readonly IAccountingContext context;
        private readonly BlobServiceClient blobServiceClient;

        public VerificationsController(IMediator mediator, IAccountingContext context, BlobServiceClient blobServiceClient)
        {
            this.mediator = mediator;
            this.context = context;
            this.blobServiceClient = blobServiceClient;
        }

        // GET: api/values
        [HttpGet]
        public async Task<VerificationsResult> GetVerificationsAsync(int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await mediator.Send(new GetVerificationsQuery(page, pageSize), cancellationToken);
        }

        [HttpGet("{verificationId}")]
        [ProducesResponseType(typeof(VerificationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VerificationDto>> GetVerificationAsync(int verificationId, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetVerificationQuery(verificationId), cancellationToken);
        }

        [HttpPost]
        public async Task<int> CreateVerification([FromBody] CreateVerification dto, CancellationToken cancellationToken)
        {
            return await mediator.Send(new CreateVerificationCommand(dto.Description, dto.InvoiceId, dto.Entries), cancellationToken);
        }

        [HttpPost("/{verificationId}/Attachment")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string?>> AddFileAttachmentToVerification(int verificationId, string? description, int? invoiceId, IFormFile file, CancellationToken cancellationToken)
        {
            return await mediator.Send(new AddFileAttachmentToVerificationCommand(verificationId, file.FileName, file.ContentType, description, invoiceId, file.OpenReadStream()), cancellationToken);
        }
    }
}