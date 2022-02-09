
using Catalog.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.Queries;

public class GetItemQuery : IRequest<ItemDto?>
{
    public string Id { get; set; }

    public GetItemQuery(string id)
    {
        Id = id;
    }

    public class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemDto?>
    {
        private readonly ICatalogContext context;
        private readonly IUrlHelper urlHelper;

        public GetItemQueryHandler(ICatalogContext context, IUrlHelper urlHelper)
        {
            this.context = context;
            this.urlHelper = urlHelper;
        }

        public async Task<ItemDto?> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) return null;

            return new ItemDto(item.Id, item.Name, item.Description, urlHelper.CreateImageUrl(item.Image!), item.CommentCount, item.Created, item.CreatedBy, item.LastModified, item.LastModifiedBy);
        }
    }
}