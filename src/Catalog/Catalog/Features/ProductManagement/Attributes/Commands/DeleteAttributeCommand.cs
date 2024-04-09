using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record DeleteAttributeCommand(string Id) : IRequest
{
    public class DeleteAttributeCommandHandler(CatalogContext context) : IRequestHandler<DeleteAttributeCommand>
    {
        public async Task Handle(DeleteAttributeCommand request, CancellationToken cancellationToken)
        {
            var attribute = await context.Attributes
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (attribute is null) throw new Exception();

            context.Attributes.Remove(attribute);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}