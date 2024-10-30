using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;
namespace YourBrand.Catalog.Features.Producers.Commands;

public sealed record CreateProducerCommand(string OrganizationId, string Name, string Handle) : IRequest<ProducerDto>
{
    public sealed class CreateProducerCommandHandler(CatalogContext context) : IRequestHandler<CreateProducerCommand, ProducerDto>
    {
        public async Task<ProducerDto> Handle(CreateProducerCommand request, CancellationToken cancellationToken)
        {
            var producer = await context.Producers
               .InOrganization(request.OrganizationId)
               .FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (producer is not null) throw new Exception();

            if (await context.Producers.AnyAsync(x => x.Name == request.Name))
            {
                throw new Exception("Producer with name already exists");
            }

            if (await context.Producers.AnyAsync(x => x.Handle == request.Handle))
            {
                throw new Exception("Handle already in use");
            }

            producer = new Domain.Entities.Producer(request.Name, request.Handle);
            producer.OrganizationId = request.OrganizationId;

            context.Producers.Add(producer);

            await context.SaveChangesAsync(cancellationToken);

            producer = await context
               .Producers
               .InOrganization(request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == producer.Id);

            return producer.ToDto();
        }
    }
}