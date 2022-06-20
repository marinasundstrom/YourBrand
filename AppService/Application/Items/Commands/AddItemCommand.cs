
using System.Data.Common;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Domain.Events;

using MediatR;

namespace YourBrand.Application.Items.Commands;

public record AddItemCommand(string Name, string Description) : IRequest
{
    public class AddItemCommandHandler : IRequestHandler<AddItemCommand>
    {
        private readonly ICatalogContext context;
        private readonly IUrlHelper urlHelper;
        private readonly IItemsClient client;

        public AddItemCommandHandler(ICatalogContext context, IUrlHelper urlHelper, IItemsClient client)
        {
            this.context = context;
            this.urlHelper = urlHelper;
            this.client = client;
        }

        public async Task<Unit> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            var item = new Item(request.Name, request.Description);

            item.AddDomainEvent(new ItemCreatedEvent(item.Id));

            context.Items.Add(item);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}