using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Producers.Commands;

public sealed record UpdateProducerCommand(string OrganizationId, int Id, string Name, string Handle) : IRequest
{
    public sealed class UpdateProducerCommandHandler(CatalogContext context) : IRequestHandler<UpdateProducerCommand>
    {
        public async Task Handle(UpdateProducerCommand request, CancellationToken cancellationToken)
        {
            var producer = await context.Producers
               .InOrganization(request.OrganizationId)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (producer is null) throw new Exception();

            producer.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}