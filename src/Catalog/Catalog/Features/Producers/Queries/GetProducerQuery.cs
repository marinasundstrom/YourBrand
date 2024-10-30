using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Producers.Queries;

public sealed record GetProducerQuery(string OrganizationId, int Id) : IRequest<ProducerDto?>
{
    sealed class GetProducerQueryHandler(
        CatalogContext context,
        IUserContext userContext) : IRequestHandler<GetProducerQuery, ProducerDto?>
    {
        public async Task<ProducerDto?> Handle(GetProducerQuery request, CancellationToken cancellationToken)
        {
            var producer = await context
               .Producers
               .InOrganization(request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (producer is null)
            {
                return null;
            }

            return producer.ToDto();
        }
    }
}