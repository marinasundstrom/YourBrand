
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.Commands;

public record UpdateItemCommand(string Id, string Name, string Description) : IRequest
{
    public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand>
    {
        private readonly IAppServiceContext context;

        public UpdateItemCommandHandler(IAppServiceContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Name = request.Name;
            item.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
