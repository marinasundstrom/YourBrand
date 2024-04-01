
using MediatR;

using YourBrand.ApiKeys.Application.Common.Interfaces;

namespace YourBrand.ApiKeys.Application.Commands;

public record CreateApiKeyCommand(string Name) : IRequest<CreateApiKeyResult>
{
    public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResult>
    {
        private readonly IApiKeysContext context;

        public CreateApiKeyCommandHandler(IApiKeysContext context)
        {
            this.context = context;
        }

        public async Task<CreateApiKeyResult> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}

public record CreateApiKeyResult(string Id, string ApiKey);