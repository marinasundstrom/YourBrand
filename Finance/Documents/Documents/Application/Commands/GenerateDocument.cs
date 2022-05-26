using Documents.Contracts;

using MassTransit;

using MediatR;

using Newtonsoft.Json;

namespace Documents.Application.Commands;

public record GenerateDocument(string TemplateId, DocumentFormat DocumentFormat, string Model) : IRequest<Stream>
{
    public class Handler : IRequestHandler<GenerateDocument, Stream>
    {
        private readonly IRequestClient<CreateDocumentFromTemplate> _requestClient;

        public Handler(IRequestClient<CreateDocumentFromTemplate> requestClient)
        {
            _requestClient = requestClient;
        }

        public async Task<Stream> Handle(GenerateDocument request, CancellationToken cancellationToken)
        {
            var result = await _requestClient.GetResponse<DocumentResponse>(
                new CreateDocumentFromTemplate(request.TemplateId, request.DocumentFormat, JsonConvert.SerializeObject(request.Model)));
            var message = result.Message;
            return new MemoryStream(await message.Document.Value);
        }
    }
}