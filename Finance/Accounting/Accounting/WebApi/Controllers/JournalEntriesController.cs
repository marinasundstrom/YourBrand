using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Application.Journal;
using YourBrand.Accounting.Application.Journal.Commands;
using YourBrand.Accounting.Application.Journal.Queries;

using Azure.Storage.Blobs;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Accounting.Controllers
{
    [Route("[controller]")]
    public class JournalEntriesController : Controller
    {
        private readonly IMediator mediator;
        private readonly IAccountingContext context;
        private readonly BlobServiceClient blobServiceClient;

        public JournalEntriesController(IMediator mediator, IAccountingContext context, BlobServiceClient blobServiceClient)
        {
            this.mediator = mediator;
            this.context = context;
            this.blobServiceClient = blobServiceClient;
        }

        // GET: api/values
        [HttpGet]
        public async Task<JournalEntryResult> GetJournalEntriesAsync(int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await mediator.Send(new GetJournalEntriesQuery(page, pageSize), cancellationToken);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JournalEntryDto>> GetJournalEntryAsync(int id, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetJournalEntryQuery(id), cancellationToken);
        }

        [HttpPost]
        public async Task<int> CreateJournalEntry([FromBody] CreateJournalEntry dto, CancellationToken cancellationToken)
        {
            return await mediator.Send(new CreateJournalEntryCommand(dto.Description, dto.InvoiceNo, dto.Entries), cancellationToken);
        }

        [HttpPost("{id}/Verifications")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string?>> AddFileToJournalEntryAsVerification(int id, string? description, int? invoiceId, IFormFile file, CancellationToken cancellationToken)
        {
            return await mediator.Send(new AddFileAsVerificationCommand(id, file.FileName, file.ContentType, description, invoiceId, file.OpenReadStream()), cancellationToken);
        }
    }
}