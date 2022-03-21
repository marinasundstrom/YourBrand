
using YourCompany.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourCompany.Application.Items.Queries;

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
            var item = await context.Items
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) return null;

            return item.ToDto(urlHelper);
         }
    }
}
