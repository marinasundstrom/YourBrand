using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Producers.Commands;

public sealed record DeleteProducerCommand(string OrganizationId, int Id) : IRequest
{
    public sealed class DeleteProducerCommandHandler(CatalogContext context) : IRequestHandler<DeleteProducerCommand>
    {
        public async Task Handle(DeleteProducerCommand request, CancellationToken cancellationToken)
        {
            var producer = await context.Producers
               .InOrganization(request.OrganizationId)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (producer is null) throw new Exception();

            context.Producers.Remove(producer);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}