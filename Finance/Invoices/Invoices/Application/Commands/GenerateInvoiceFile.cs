using Documents.Client;

using Invoices.Application;
using Invoices.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Invoices.Application.Commands;

public record GenerateInvoiceFile(int InvoiceId) : IRequest<Stream>
{
    public class Handler : IRequestHandler<GenerateInvoiceFile, Stream>
    {
        private readonly IInvoicesContext _context;
        private readonly IDocumentsClient _documentsClient;

        public Handler(IInvoicesContext context, IDocumentsClient documentsClient)
        {
            _context = context;
            _documentsClient = documentsClient;
        }

        public async Task<Stream> Handle(GenerateInvoiceFile request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if(invoice is null) 
            {
                throw new Exception();
            }

            var model = JsonConvert.SerializeObject(
                    JsonConvert.SerializeObject(invoice.ToDto()).ToString());

            Console.WriteLine(model);

            var response = await _documentsClient.GenerateDocumentAsync("invoice", DocumentFormat.Html, model);
            return response.Stream;
        }
    }
}