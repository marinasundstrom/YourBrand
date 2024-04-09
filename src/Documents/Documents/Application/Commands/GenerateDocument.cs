using MassTransit;

using MediatR;

using Newtonsoft.Json;

using YourBrand.Documents.Contracts;

namespace YourBrand.Documents.Application.Commands;

public record GenerateDocument(string TemplateId, DocumentFormat DocumentFormat, string Model) : IRequest<Stream>
{
    public class Handler(IRequestClient<CreateDocumentFromTemplate> requestClient) : IRequestHandler<GenerateDocument, Stream>
    {
        public async Task<Stream> Handle(GenerateDocument request, CancellationToken cancellationToken)
        {
            var result = await requestClient.GetResponse<DocumentResponse>(
                new CreateDocumentFromTemplate(request.TemplateId, request.DocumentFormat, JsonConvert.SerializeObject(request.Model)));
            var message = result.Message;
            return new MemoryStream(await message.Document.Value);
        }
    }
}