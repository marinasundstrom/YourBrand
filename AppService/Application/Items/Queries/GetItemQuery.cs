
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Queries;

public record GetItemQuery(string Id) : IRequest<ItemDto?>
{
    public class GetItemQueryHandler : IRequestHandler<GetItemQuery, ItemDto?>
    {
        private readonly IAppServiceContext context;
        private readonly IUrlHelper urlHelper;

        public GetItemQueryHandler(IAppServiceContext context, IUrlHelper urlHelper)
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
