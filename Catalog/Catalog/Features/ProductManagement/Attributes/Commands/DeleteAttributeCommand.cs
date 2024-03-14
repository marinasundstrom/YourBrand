using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record DeleteAttributeCommand(string Id) : IRequest
{
    public class DeleteAttributeCommandHandler : IRequestHandler<DeleteAttributeCommand>
    {
        private readonly CatalogContext context;

        public DeleteAttributeCommandHandler(CatalogContext context)
        {
            this.context = context;
        }

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